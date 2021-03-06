﻿
C$game = 1;

C$game$width = 0;
C$game$height = 0;
C$game$pwidth = 0;
C$game$pheight = 0;
C$game$fps = 60;
C$game$real_canvas = null;
C$game$virtual_canvas = null;
C$game$scaled_mode = false;
C$game$ctx = null;
C$game$execId = -1;

C$game$now = function() { return (Date.now ? Date.now() : new Date().getTime()) / 1000.0; };
C$game$last_frame_began = C$game$now();

C$game$beginFrame = function () {
    C$game$last_frame_began = C$game$now();
};

C$game$knownSize = null;

C$game$platformSpecificEnforceFullScreen = function(jsCanvas, canvasCtx, windowSize, gameSize) {
    // TODO: make this a no-op and then move this to iOS
    jsCanvas.width = windowSize[0];
    jsCanvas.height = windowSize[1];
    canvasCtx.scale(screen[0] / gameSize[0], screen[1] / gameSize[1]);
};

C$game$getScreenDimensionsForFullScreen = function() {
  return [window.innerWidth, window.innerHeight];
};

C$game$enforceFullScreen = function() {
	var screen = C$game$getScreenDimensionsForFullScreen();
	if (C$game$knownSize === null ||
		C$game$knownSize[0] != screen[0] ||
		C$game$knownSize[1] != screen[1]) {
		
		var phsCanvas = C$game$real_canvas;
		C$game$knownSize = screen;
		C$game$platformSpecificEnforceFullScreen(phsCanvas, C$game$ctx, screen, [C$game$width, C$game$height]);
	}
};

C$game$endFrame = function() {
    if (C$game$scaled_mode) {
        C$game$real_canvas.getContext('2d').drawImage(C$game$virtual_canvas, 0, 0);
    }
    if (!!C$common$globalOptions['fullscreen']) {
		C$game$enforceFullScreen();
    }
    
    C$game$everyFrame();
    window.setTimeout(C$game$runFrame, C$game$computeDelayMillis());
};

C$game$runFrame = function () {
    C$game$beginFrame();
    C$handleVmResult(runInterpreter(C$common$programData, C$game$execId)); // clockTick will induce endFrame()
};

C$game$computeDelayMillis = function () {
    var ideal = 1.0 / C$game$fps;
    var diff = C$game$now() - C$game$last_frame_began;
    var delay = Math.floor((ideal - diff) * 1000);
    if (delay < 1) delay = 1;
    return delay;
};

C$game$initializeGame = function (fps) {
    C$game$fps = fps;
};

C$game$pumpEventObjects = function () {
  var newEvents = [];
  var output = C$input$eventRelays;
  C$input$eventRelays = newEvents;
  return output;
};

C$game$everyFrame = function() {
// override in other platforms
};

C$game$wrapCanvasHtml = function(canvasHtml) {
    return canvasHtml;
};

// TODO: also apply keydown and mousedown handlers
// TODO: (here and python as well) throw an error if you attempt to call this twice.
C$game$initializeScreen = function (width, height, pwidth, pheight, execId) {
  var scaledMode;
  var canvasWidth;
  var canvasHeight;
  var virtualCanvas = null;
  var screenSize = C$game$enforcedScreenSize(pwidth, pheight);
  pwidth = screenSize[0];
  pheight = screenSize[1];
  if (pwidth === null || pheight === null) {
    scaledMode = false;
    canvasWidth = width;
    canvasHeight = height;
  } else {
    scaledMode = true;
    canvasWidth = pwidth;
    canvasHeight = pheight;
    virtualCanvas = document.createElement('canvas');
    virtualCanvas.width = width;
    virtualCanvas.height = height;
  }
  
  var innerHost = C$game$getCrayonHostInner();
  
  // make sure the font loader exists first so that it can hide behind the screen.
  C$game$getFontLoader();
  innerHost.innerHTML +=
	C$game$wrapCanvasHtml('<canvas id="crayon_screen" width="' + canvasWidth + '" height="' + canvasHeight + '"></canvas>') +
	'<div style="display:none;">' +
		'<img id="crayon_image_loader" crossOrigin="anonymous" />' +
		'<div id="crayon_image_loader_queue"></div>' +
		'<div id="crayon_image_store"></div>' +
		'<div id="crayon_temp_image"></div>' +
		'<div id="crayon_font_loader"></div>' +
	'</div>';
  var canvas = document.getElementById('crayon_screen');

  C$game$scaled_mode = scaledMode;
  C$game$real_canvas = canvas;
  C$game$virtual_canvas = scaledMode ? virtualCanvas : canvas;
  C$game$ctx = canvas.getContext('2d');
  C$game$width = width;
  C$game$height = height;
  C$game$execId = execId;

  C$images$image_loader = document.getElementById('crayon_image_loader');
  C$images$image_store = document.getElementById('crayon_image_store');
  C$images$temp_image = document.getElementById('crayon_temp_image');

  document.onkeydown = C$input$keydown;
  document.onkeyup = C$input$keyup;

  canvas.addEventListener('mousedown', C$input$mousedown);
  canvas.addEventListener('mouseup', C$input$mouseup);
  canvas.addEventListener('mousemove', C$input$mousemove);

  if (C$game$isMobile()) {
	canvas.ontouchstart = C$game$onTouchUpdate;
	canvas.ontouchmove = C$game$onTouchUpdate;
	canvas.ontouchcancel = C$game$onTouchUpdate;
	canvas.ontouchend = C$game$onTouchUpdate;
  }

  C$game$ctx.imageSmoothingEnabled = false;
  C$game$ctx.mozImageSmoothingEnabled = false;
  C$game$ctx.msImageSmoothingEnabled = false;
  C$game$ctx.webkitImageSmoothingEnabled = false;

  if (scaledMode) {
      C$game$ctx.scale(pwidth / width, pheight / height);
  }

  window.setTimeout(C$game$runFrame, 0);
};

C$game$doRender = function() {
	C$drawing$render(C$game$ctx, C$game$width, C$game$height);
};

C$game$getCrayonHostInner = function() {
  var d = document.getElementById('crayon_host_inner');
  if (!d) {
	document.getElementById('crayon_host').innerHTML = '<div style="position:relative;" id="crayon_host_inner"></div>';
	d = document.getElementById('crayon_host_inner');
  }
  return d;
};

C$game$getFontLoader = function() {
  var host = C$game$getCrayonHostInner();
  var id = 'crayon_font_loader_host';
  var fl = document.getElementById(id);
  if (!fl) {
    host.innerHTML += 
      '<div style="position:absolute;left:0px;top:0px;font-size:8px" id="' + id + '">' + 
	    '<span id="crayon_font_loader_1"></span>' +
	    '<span id="crayon_font_loader_2"></span>' +
	  '</div>';
	fl = document.getElementById(id);
  }
  return fl;
};

C$game$setTitle = function (title) {
  window.document.title = title;
};

C$game$enforcedScreenSize = function(width, height) { return [width, height]; };

C$game$screenInfo = function(o) { return false; };

window.addEventListener('keydown', function(e) {
  if ([32, 37, 38, 39, 40].indexOf(e.keyCode) > -1) {
    e.preventDefault();
  }
}, false);

C$game$touches = {}; // { ID -> [x, y] }

C$game$onTouchUpdate = function(ev) {
  ev.preventDefault();
  C$game$touches = {};
  var w = C$game$width;
  var h = C$game$height;
  for (var i = 0; i < ev.touches.length; ++i) {
	var t = ev.touches[i];
	var c = t.target;
	var x = Math.floor(w * (t.clientX - c.clientLeft) / c.clientWidth);
	var y = Math.floor(h * (t.clientY - c.clientTop) / c.clientHeight);
	x = Math.max(0, Math.min(w - 1, x));
	y = Math.max(0, Math.min(h - 1, y));
	C$game$touches[t.identifier] = [x, y];
  }
};

C$game$getTouchState = function(data) {
	var n = 0;
	for (var key in C$game$touches) {
		var p = C$game$touches[key];
		var i = n * 3 + 1;
		data[i] = key;
		data[i + 1] = p[0];
		data[i + 2] = p[1];
		n++;
		if (n > 9) break;
	}
	data[0] = n;
};

C$game$isMobile = function() {
	return navigator.userAgent.indexOf('Mobile') !== -1;
};

C$game$getCallbackFunctions = function() {
	var t = {};
	t["set-render-data"] = function(o) { return C$drawing$rendererSetData(o[0], o[1], o[2], o[3]); };
	return t;
};
