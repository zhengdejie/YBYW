(function(){
var isIOS = /(iPad|iPhone|iPod)/g.test( navigator.userAgent );
var isAndroid = /Android/g.test( navigator.userAgent );
var isInWebFrame = /WebFrame/g.test( navigator.userAgent );
var webframe = {};
webframe.getBridge = function(callback) {
  if (window.WebViewJavascriptBridge)
    callback(WebViewJavascriptBridge);
  else
    document.addEventListener('WebViewJavascriptBridgeReady', function() {
      callback(WebViewJavascriptBridge)
    }, false);
};
// init bridge
webframe.getBridge(function(b) {
  b.init(function(message, responseCallback) {
    responseCallback({})
  })
});
var getAbsoluteUrl = (function() {
	var a;

	return function(url) {
		if(/\:/.test(url)) return url;
		if(!a) a = document.createElement('a');
		a.href = url;

		return a.href;
	};
})();
webframe.openUrl = function(url, where){
	var realUrl = getAbsoluteUrl(url);
	if(/^http/i.test(realUrl)){
		if(!where) where = "new";
	}else{
		// 对于 不是http/https where 无效
		where = "";
	}
	var prefix = "";
	if(where == "new")
		prefix = "wf-";
	else if(where == "modal")
		prefix = "wf-m-";
	else if(where == "replace")
		prefix = "wf-r-";
	webframe.getBridge(function(b){
		b.callHandler('openUrl', prefix + realUrl);
	});
};
var actionButtonClickedCB;

webframe.getBridge(function(b){
	b.registerHandler('onActionButtonClicked', function(data, cb){
		if(actionButtonClickedCB)
			actionButtonClickedCB(data);
	});
});
webframe.setActionButton = function(button){
	webframe.getBridge(function(b){
		b.callHandler('setActionButton', button);
	});
};
webframe.removeActionButton = function(buttonID){
	webframe.getBridge(function(b){
		b.callHandler('removeActionButton', buttonID);
	});
};
webframe.onActionButtonClicked = function(cb){
	actionButtonClickedCB = cb;
};
var childPageFinishedCB;

webframe.getBridge(function(b){
	b.registerHandler('onChildPageFinished', function(data, cb){
		if(childPageFinishedCB)
			childPageFinishedCB(data);
	});
});
webframe.onChildPageFinished = function(cb){
	childPageFinishedCB = cb;
};
webframe.finish = function(data){

	webframe.getBridge(function(b){
		b.callHandler('finish', data);
	});
};

// w => previous, all
webframe.setWebViewAutoReload = function(w){
	if(!w) w = "previous";

	webframe.getBridge(function(b){
		b.callHandler('setWebViewAutoReload', w);
	});
};
webframe.getAppConfig = function(key, cb){
	webframe.getBridge(function(b){
		b.callHandler('getAppConfig', key, cb);
	});
};
webframe.setAppConfig = function(key, value){
	webframe.getBridge(function(b){
		b.callHandler('setAppConfig', {"key": key, "value": value});
	});
};
// TODO
// http://ipinfo.io/?callback=callback
webframe.requestLocation = function(options, callback){
	if((typeof options == 'function') && !callback){
		callback = options;
		options = {};
	}
	webframe.getBridge(function(b){
		b.callHandler('requestLocation', options, callback);
	});
};


webframe.showActionSheet = function(options, callback){
	webframe.getBridge(function(b){
		b.callHandler('showActionSheet', options, callback);
	});	
};

webframe.showAlert = function(options, callback){
	webframe.getBridge(function(b){
		b.callHandler('showAlert', options, callback);
	});	
};

webframe.showInput = function(options, callback){
	webframe.getBridge(function(b){
		b.callHandler('showInput', options, callback);
	});	
};
function emptyAlertCallback(w){}
webframe.alert = function(msg, callback){
	callback = callback || emptyAlertCallback;
	if(isInWebFrame && isAndroid){
		// android 我们做了 native-alert
		alert(msg);
		callback(null);
	}else if(isInWebFrame){
		this.showAlert({'title': msg, buttonCancel: '确定'}, callback || emptyAlertCallback);
	}else{
		// desktop
		alert(msg);
		callback(null);
	}
};
webframe.confirm = function(msg, callback){
	if(isInWebFrame)
		this.showAlert({'title': msg, buttonCancel: '取消', buttons: [{title: '确定', id: '1'}]}, callback);
	else{
		var a = confirm(msg);
		if(!a) a = null;
		else a = '1';
		callback(a);
	}
};
webframe.prompt = function(msg, _default, callback){
	this.promptType('text', msg, _default, callback);
};
webframe.promptType = function(type, msg, _default, callback){
	if(isInWebFrame){
		var m1 = type.match(/^(.*)\,(\d+)$/);
		if(m1){
			this.showInput({'title': msg, inputType: m1[1], inputMultiline: parseInt(m1[2]), inputDefault: _default}, callback);
			return;
		}
		this.showInput({'title': msg, inputType: type, inputDefault: _default}, callback);
	}else{
		var a = prompt(msg, _default);
		callback(a);
	}
};
webframe.copyText = function(str){
	webframe.getBridge(function(b){
		b.callHandler('copyText', str);
	});	
};

webframe.onLoaded = function(func){
	if(window.wfRequestHeaders){
		func();
		return;
	}
	var i = setInterval(function(){
		if(window.wfRequestHeaders){
			clearInterval(i);
			func();
		}
	}, 40);
};
window.webframe = webframe;


// jquery
if($){
	$.ajaxSetup({
		beforeSend: function (xhr) {
			if(window.wfRequestHeaders)
				for(var key in window.wfRequestHeaders)
					xhr.setRequestHeader(key, window.wfRequestHeaders[key]);  
		}
	});
	$( document ).ajaxError(function() {
		webframe.alert("出现错误，请检查网络");
	});
}
})();
