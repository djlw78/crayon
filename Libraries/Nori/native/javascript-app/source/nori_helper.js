
function platformSpecificHandleEvent(id, type, args) {
	if (NoriHelper.vm === null) {
		NoriHelper.eventQueue.push([id, type, args]);
	} else {
		var globals = vmGetGlobals(NoriHelper.vm);
		var vmFunctionPointerArgs = [
			NoriHelper.frameValueHack,
			buildInteger(globals, id),
			buildString(globals, type),
			buildString(globals, args)
		];
		runInterpreterWithFunctionPointer(
			NoriHelper.vm,
			NoriHelper.eventHandlerCallback,
			vmFunctionPointerArgs);
	}
}

var NoriHelper = {
	eventQueue: [],
	eventHandlerCallback: null,
	vm: null,
	
	// TODO: this needs to be a dictionary and frame values need an ID#.
	// For JavaScript apps, this will all run in the same process, most likely.
	// For others, there is no collision.
	frameValueHack: null,
	
};

NoriHelper.getCrayonHost = function() {
	return document.getElementById('crayon_host');
};

NoriHelper.getWindowSize = function() {
	var width = window.innerWidth;
	var height = window.innerHeight;
	if (C$common$globalOptions['is_android']) {
		width = Math.floor(width / 2);
		height = Math.floor(height / 2);
	}
	return [width, height];
};

NoriHelper.ShowFrame = function(crayonFrameValue, title, width, height, data, execId) {
	NoriHelper.frameValueHack = crayonFrameValue;
	var ch = NoriHelper.getCrayonHost();
	
	while (ch.lastChild) {
		ch.removeChild(ch.lastChild);
	}
	
	ch.style.height = '100%';
	ch.style.backgroundColor = '#ffffff';
	
	var widthHeight = NoriHelper.getWindowSize();
	width = widthHeight[0];
	height = widthHeight[1];

	if (!!window.sendMessage) {
		sendMessage("view-type", "natural-web-view");
	}

	// for browsers, width and height are ignored.
	ch.style.width = width + 'px';
	ch.style.height = height + 'px';
	
	var body = ch.parentElement;
	body.style.margin = '0px';
	
	var nh = document.createElement('div');
	nh.style.width = '100%';
	nh.style.height = '100%';
	nh.style.position = 'relative';
	nh.crayon_execId = execId;
	nh.crayon_frameValue = crayonFrameValue;
	
	// TODO: wrap all contents of nori.js into a single object instance.
	ch.appendChild(nh);
	setFrameRoot(nh);
	setFrameSize(width, height);
	
	nh.style.overflowX = 'hidden';
	nh.style.overflowY = 'hidden';
	
	// update flushing must occur AFTER the elements are added to the document.
	// Some of the layout stuff (such as text size calculation) requires the elements
	// to be in the document.
	flushUpdates(data);
	
	window.onresize = function() {
		var widthHeight = NoriHelper.getWindowSize();
		var w = widthHeight[0];
		var h = widthHeight[1];
		
		var arg = w + ',' + h;
		// TODO: set a timeout and rate-limit the callbacks. Rate limiter should be in nori.js.
		
		platformSpecificHandleEvent(-1, 'frame.onresize', arg);
		setFrameSize(w, h);
		ch.style.width = w + 'px';
		ch.style.height = h + 'px';
		flushUpdates("NO,0"); // no-op. induces a UI refresh.
	};
	
	return nh;
};

NoriHelper.CloseFrame = function(frameHandle) {
	var ch = NoriHelper.getCrayonHost();
	
	while (ch.lastChild) {
		ch.removeChild(ch.lastChild);
	}
};

NoriHelper.FlushUpdatesToFrame = function(frameHandle, uiData) {
	var ch = document.getElementById('crayon_host');
	if (ch.firstChild !== frameHandle) throw "Multiple frames not supported";
	// TODO: open iframes to determine proper frame. For now, assume one frame.
	flushUpdates(uiData);
};

NoriHelper.HEX = '0123456789ABCDEF';
NoriHelper.EscapeStringHex = function(original) {
	var output = [];
	var h = NoriHelper.HEX;
	var len = original.length;
	var c;
	for (var i = 0; i < len; ++i) {
		c = original.charCodeAt(i);
		output.push(h.charAt((c >> 4) & 255));
		output.push(h.charAt(c & 15));
	}
	var t= output.join('');
	return t;
};

NoriHelper.EventWatcher = function(vm, execContextIdForResume, frameAsValue, eventCallback, postShowCallback) {
	NoriHelper.vm = vm;
	NoriHelper.eventHandlerCallback = eventCallback;
	runInterpreterWithFunctionPointer(
		NoriHelper.vm,
		postShowCallback,
		[frameAsValue]);
	if (NoriHelper.eventQueue.length > 0) {
		for (var i = 0; i < NoriHelper.eventQueue.lengt; ++i) {
			var e = NoriHelper.eventQueue[i];
			platformSpecificHandleEvent(e[0], e[1], e[2]);
		}
		NoriHelper.eventQueue = [];
	}
};

NoriHelper.imagesByIdHoldingArea = {};

NoriHelper.SendImageToRenderer = function(frameObj, id, nativeImageData, x, y, width, height) {
	var newImage = document.createElement('canvas');
	var c = newImage.getContext('2d');
	newImage.width = width;
	newImage.height = height;
	c.drawImage(nativeImageData, -x, -y);
	
	NoriHelper.imagesByIdHoldingArea[id] = newImage;
};

function getImageInHoldingArea(id) {
	var lookup = NoriHelper.imagesByIdHoldingArea;
	var img = lookup[id];
	delete lookup[id];
	return img;
}
