var MOUSEWHEEL_EVT = (/Firefox/i.test(navigator.userAgent)) ? "DOMMouseScroll" : "mousewheel" //FF doesn't recognize mousewheel as of FF3.x

var IMAGE_MARGIN = 50,
    WORKSTATION_RADIUS = 6,
    WORKSTATION_STROKE = 2;

var TYPE_SPACE = 0,
    TYPE_WORKSTATION = 1;

var CAN_ZOOM = true;

var Canvas = null,
    ImageObj = null,
    Pin = null;

var PinnedObj = null;
function PlacePin(obj) {
    if (PinnedObj) PinnedObj.pinned = undefined;
    PinnedObj = obj;
    obj.pinned = true;

    ZoomPin();
    Pin.opacity = 1;
    Pin.bringToFront();
};
function ZoomPin() {
    if (PinnedObj == null) return;

    var point;
    if (PinnedObj.type == 'rect') {
        point = rectGetCentre(
            {
                X: PinnedObj.left,
                Y: PinnedObj.top
            },
            {
                X: PinnedObj.left + PinnedObj.getWidth(),
                Y: PinnedObj.top + PinnedObj.getHeight()
            });
    } else if (PinnedObj.type == 'circle') {
        point = {
            X: PinnedObj.left + (WORKSTATION_RADIUS + WORKSTATION_STROKE) * AbsoluteScale,
            Y: PinnedObj.top + (WORKSTATION_RADIUS + WORKSTATION_STROKE) * AbsoluteScale
        };
    } else {
        return;
    }

    Pin.top = point.Y - 32; // half of pin img height...
    Pin.left = point.X - 9; // half of pin img width...
}
function HidePin() {
    if (Pin !== null)
    {
        Pin.opacity = 0;
        Canvas.renderAll();
    }
};

var AddingObject = false;
var currentLocationId = 0;
function CanvasLoaded() { return Canvas !== null; }

var AbsoluteScale = 1;

$().ready(function () {
    $('.floating-container')
        .mouseover(function () { CAN_ZOOM = false; })
        .mouseout(function () { CAN_ZOOM = true; });

    $('*').bind(MOUSEWHEEL_EVT, Scroll);
});

function Scroll(e) {
    if (!CanvasLoaded() || !CAN_ZOOM) {
        return true;
    }

    var evt = window.event || e; //equalize event object     
    evt = evt.originalEvent ? evt.originalEvent : evt; //convert to originalEvent if possible               
    var delta = evt.detail ? evt.detail * (-40) : evt.wheelDelta; //check for detail first, because it is used by Opera and FF

    var dy = delta > 0 ? 1 : -1;
    Scrolling = dy;
    setTimeout(function () {
        zoom(dy);
    }, 0);
    return false;
}

var ResizeCanvas;

function LoadCanvas(filename, spaces, callback) {
    if (CanvasLoaded()) {
        $('.upper-canvas').remove();
        removeAllObjects();
    }

    var async = 1;
    var timeout = 1800;
    fabric.Image.fromURL('Images/' + filename, function (imgObj) {
        if (imgObj._originalElement == null) {
            async--
            timeout = -1;
            return;
        }

        AbsoluteScale = 1;
        Canvas = new fabric.Canvas('mainCanvas');
        $('#canvasWrapper').find('*').bind(MOUSEWHEEL_EVT, Scroll);
        ResizeCanvas = function () {
            // resize canvas according to the image
            Canvas.setHeight(imgObj.getHeight() + IMAGE_MARGIN * 2);
            Canvas.setWidth(imgObj.getWidth() + IMAGE_MARGIN * 2);

            Main.ResizeCanvasWrapper();
        };
        ResizeCanvas();

        var pinElem = document.getElementById('PinImage');
        Pin = new fabric.Image(pinElem, {
            selectable: false
        });
        Canvas.add(Pin);
        HidePin();

        ImageObj = imgObj;
        imgObj.selectable = false;
        imgObj.left = IMAGE_MARGIN;
        imgObj.top = IMAGE_MARGIN;
        imgObj.opacity = 0.80;
        Canvas.add(imgObj);
        ImageObj.sendToBack();

        Canvas.observe('mouse:down', function (e) { mousedown(e); });
        Canvas.observe('mouse:move', function (e) { mousemove(e); });
        Canvas.observe('mouse:up', function (e) { mouseup(e); });
        Canvas.selection = false;

        /*Canvas.on('mouse:over', function (options) {
            if (options.target) {
                // we dont care about hovering over the image
                if (options.target.type != 'image') {
                    mouseover(options.target);
                }
            }
        });

        Canvas.on('mouse:out', function (options) {
            if (options.target) {
                // we dont care about hovering over the image
                if (options.target.type != 'image') {
                    mouseout(options.target);
                }
            }
        });*/

        async--;
    });

    (function go() {
        if (timeout < 0) {
            callback(false);
        } else if (async > 0) {
            timeout -= 50;
            setTimeout(go, 50);
        } else {
            RenderObjects(spaces);
            callback(true);
        }
    })();
}

function RenderObjects(spaces) {
    $.each(spaces || [], function (i, s) {
        /*var textObj = createTextObj(s.Name, s.TopLeft.X, s.TopLeft.Y);
        Canvas.add(textObj);
        drawRect(s.TopLeft.X, s.TopLeft.Y, s.BottomRight.X - s.TopLeft.X, s.BottomRight.Y - s.TopLeft.Y)
            .set('w', s).set('text', textObj);
        $.each(s.Workstations || [], function (j, w) {
            drawCircle(s.locationId, w.Point.X, w.Point.Y)
                .set('w', w);*/
        drawCircle(s, s.EntityCoordinates[0].X, s.EntityCoordinates[0].Y);
    });
    Canvas.sendToBack(ImageObj);
    ImageObj.sendToBack();
    Canvas.renderAll();
}

//-------------------------------------------
// Mouse interactions with the canvas
/*var T_SELECT = 0,
    T_DRAW_RECT = 1,
    T_DRAW_POLY = 2,
    T_TAG_WORKSTATION = 3,
    T_ERASE = 4;*/
var startedDrag = false;
var startedPanning = false;
var x = 0;
var y = 0;
var originalX = 0;
var originalY = 0;

function drawRect(x, y, w, h) {
    var square = new fabric.Rect({
        width: w,
        height: h,
        left: x,
        top: y,
        opacity: 0.5,
        fill: 'white',
        stroke: '#aaf',
        strokeWidth: 5 * AbsoluteScale,
        selectable: false,
        lockMovementX: true,
        lockMovementY: true,
        hasControls: false,
        hoverCursor: 'default',
        cornerColor: 'green',
        cornerSize: 10
    });
    //square.observe('mouse:over', function (e) { mouseover(e, this); });
    Canvas.add(square);
    Canvas.renderAll();
    Canvas.setActiveObject(square);

    return square;
}

function drawCircle(locationObj, x, y) {
    var circle = new fabric.Circle({
        locationObj: locationObj,
        radius: WORKSTATION_RADIUS * AbsoluteScale,
        left: x - (7 * AbsoluteScale),
        top: y - (7 * AbsoluteScale),
        opacity: 0.75,
        fill: 'red',
        stroke: '#aaf',
        strokeWidth: WORKSTATION_STROKE * AbsoluteScale,
        selectable: false,
        lockMovementX: true,
        lockMovementY: true,
        hasControls: false,
        hoverCursor: 'default',
        cornerColor: 'green'
    });
    Canvas.add(circle);
    Canvas.bringToFront(circle);
    Canvas.renderAll();
    Canvas.setActiveObject(circle);

    return circle;
}

function createTextObj(text, x, y) {
    return new fabric.Text(text, {
        left: x + 6, top: y + 6, fontFamily: 'Arial', fontSize: 17, fontWeight: 'bold', fill: '#9191FF'
    });

}

/* Mousedown */
function mousedown(e) {
    var obj = e.target;
    var mouse = Canvas.getPointer(e.e);
    console.log("Mousedown x , y");
    console.log(mouse.x);
    console.log(mouse.y);
    switch (CurrentContext.CurrentTool()) {
        case T_DRAW_RECT:
            startedDrag = true;
            originalX = mouse.x;
            originalY = mouse.y;
            x = originalX;
            y = originalY;
            drawRect(x, y, 0, 0);
            break;
        case T_TAG_WORKSTATION:
            x = mouse.x;
            y = mouse.y;
            //if (obj !== undefined && obj.type == 'rect') {
                drawCircle(null, x, y);
            //} else {
            //    alert("Workstations can only be placed within marked spaces. Please define the room before attempting to add the workstation.");
            //}
            break;
        case T_SELECT:
            startedPanning = true;
            originalX = mouse.x;
            originalY = mouse.y;
            break;
    }
}

/* Mousemove */
function mousemove(e) {
    if (CurrentContext.CurrentTool() == T_DRAW_RECT) {
        if (!startedDrag) {
            return false;
        }
        var w, h;
        var mouse = Canvas.getPointer(e.e);
        //Mouse moved to the left, change top left of box
        if (mouse.x < originalX) {
            w = originalX - mouse.x;
            x = mouse.x;
            //Mouse is to the right of the box still
        } else {
            w = mouse.x - originalX - 5;
            x = originalX;
        }

        //Mouse moved to the top, change top left of box
        if (mouse.y < originalY) {
            h = originalY - mouse.y;
            y = mouse.y;
            //Mouse is to the bottom of the box still
        } else {
            h = mouse.y - originalY - 5;
            y = originalY;
        }

        if (!w || !h) {
            return false;
        }

        var square = Canvas.getActiveObject();
        square.set('left', x).set('top', y);
        square.set('width', w).set('height', h);
        Canvas.renderAll();
    }
    else if (CurrentContext.CurrentTool() == T_SELECT){
        if (!startedPanning){
            return false;//can ignore because just moving mouse without a mousedown first
        }
        var mouse = Canvas.getPointer(e.e);
        var deltaX, deltaY;
        deltaX = originalX - mouse.x;
        deltaY = originalY - mouse.y;
        originalX = mouse.x;
        originalY = mouse.y;
        shiftMap(deltaX, deltaY);
    }
}

/* Mouseup */
function mouseup(e) {
    //need to open popup window and save

    var obj = e.target;
    switch (CurrentContext.CurrentTool()) {
        case T_SELECT:
            if (startedPanning){
                startedPanning = false;
            }
            if (obj !== undefined && obj !== null) {
                /*if (obj.type == 'rect') {
                    obj.sendToBack();
                    obj.text.sendToBack();
                    ImageObj.sendToBack();
                    Main.ShowDetails('Space', obj.w.LocationId);
                } else */
                if (obj.type == 'circle') {
                    obj.bringToFront();
                    Main.ShowDetails('Workstation', obj.locationObj.EntityId);
                }
            }
            break;
        case T_DRAW_RECT:
            if (startedDrag) {
                startedDrag = false;
            }
            Canvas.renderAll();
            //(locationId, w, topLeftPoint, bottomRightPoint, callback)
            var activeObject = Canvas.getActiveObject();
            SpaceDialog.open(0, null, { X: Math.round(Canvas.getActiveObject().left), Y: Math.round(Canvas.getActiveObject().top) }, { X: Math.round(Canvas.getActiveObject().left + Canvas.getActiveObject().width), Y: Math.round(Canvas.getActiveObject().top + Canvas.getActiveObject().height) }, function (w) {
                if (w == null) {
                    //not succesful, delete the obj
                    Canvas.remove(activeObject);
                } else {
                    //success - 
                    var text = createTextObj(w.Name, activeObject.left, activeObject.top);
                    Canvas.add(text);
                    text.sendToBack();
                    ImageObj.sendToBack();
                    activeObject.set('w', w).set('text', text);
                }
            });
            break;
        case T_TAG_WORKSTATION:
            AddingObject = true;
            EntityDialog.open(Canvas.getActiveObject().parentLocationId, null, { X: Math.round(Canvas.getActiveObject().left), Y: Math.round(Canvas.getActiveObject().top) }, function (w) {
                if (w == null) {
                    //not succesful, delete the obj
                    Canvas.remove(Canvas.getActiveObject());
                } else {
                    //success - 
                    Canvas.getActiveObject().set('w', w);
                }
            });
            break;
        case T_ERASE:
            //var obj = Canvas.getActiveObject();
            if (obj && obj.type != 'img') {
                if (obj.type == 'circle') {
                    removeWorkstation(obj);
                } else if (obj.type == 'rect') {
                    removeSpace(obj);
                }
            }
            break;
    }

    //makes rectangles able to be selected/hovered over well
    zoom(0);
}

function removeWorkstation(obj) {
    if (confirm("Are you sure you want to delete this workstation?")) {
        //Temporary Stub to allow for deleting without any server calls
        if (CurrentContext.RemoveWorkstation(obj.locationObj.EntityId)) {
            Canvas.remove(obj);
            HidePin();
            SearchResultsDetailsCard.clear();
        } else {
            alert('Unable to remove the workstation');
        }
    }
}

function removeSpace(obj) {
    if (confirm("Are you sure you want to delete this room?")) {
        if (CurrentContext.RemoveSpace(obj.w.LocationId)) {
            var workstations = getObjectsByParentId(obj.w.LocationId);
            $.each(workstations, function (i, w) {
                Canvas.remove(w);
            });
            Canvas.remove(obj.text);
            Canvas.remove(obj);
            SearchResultsDetailsCard.clear();
        } else {
            alert('Unable to remove the space');
        }
    }
}

//Hover stuff - dont touch
function mouseover(obj) {
    //obj.stroke = 'black';
    //alert(obj.w.locationId);
    console.log(obj.type);
}

function mouseout(obj) {
    //obj.stroke = '#aaf';
}

var ZoomStep = 0.05;
var Scrolling = 0;
function zoom(relativeZoomLevel) {
    if (!CanvasLoaded()) {
        return;
    }

    // TODO limit the max canvas zoom in
    // cancel events if user changes direction
    if (Math.abs(relativeZoomLevel + Scrolling) !== (Math.abs(relativeZoomLevel) + Math.abs(Scrolling)))
        return;

    var scaleFactor = 1 + relativeZoomLevel * ZoomStep;
    AbsoluteScale += relativeZoomLevel * ZoomStep;
    //var canvasScale = canvasScale * ScaleFactor;

    Canvas.setHeight(Canvas.getHeight() * scaleFactor);
    Canvas.setWidth(Canvas.getWidth() * scaleFactor);

    var objects = Canvas.getObjects();
    for (var i in objects) {
        var obj = objects[i];

        if (obj == Pin) continue;

        var scaleX = obj.scaleX;
        var scaleY = obj.scaleY;
        var left = obj.left;
        var top = obj.top;

        var tempScaleX = scaleX * scaleFactor;
        var tempScaleY = scaleY * scaleFactor;
        var tempLeft = left * scaleFactor;
        var tempTop = top * scaleFactor;

        obj.scaleX = tempScaleX;
        obj.scaleY = tempScaleY;
        obj.left = tempLeft;
        obj.top = tempTop;

        obj.setCoords();
    }

    ZoomPin();

    ResizeCanvas();
    Canvas.renderAll();
}

function removeAllControls() {
    var objects = Canvas.getObjects();
    for (var i in objects) {
        objects[i].hasControls = false;
        objects[i].lockMovementX = true;
        objects[i].lockMovementY = true;
        objects[i].selectable = false;
    }
}

function removeAllObjects() {
    var objects = Canvas.getObjects();
    for (var i in objects) {
        Canvas.remove(objects[i]);
    }
}

function removeObjectByLocationId(locationId) {
    var obj = getObjectByLocationId(locationId);
    if (obj == null) return;

    Canvas.remove(obj);
}

function selectObjectByLocationId(locationId) {
    var obj = getObjectByLocationId(locationId);
    if (obj == null) return;

    Canvas.setActiveObject(obj);
}

function getObjectByLocationId(locationId) {
    var objects = Canvas.getObjects();
    for (var i in objects) {
        if (objects[i].locationObj != undefined && objects[i].locationObj.EntityId == locationId) {
            return objects[i];
        }
    }

    return null;
}

function getObjectsByParentId(parentId) {
    var objects = Canvas.getObjects();
    var output = [];
    for (var i in objects) {
        if (objects[i].w != undefined && objects[i].w.parentLocationId == parentId) {
            output.push(objects[i]);
        }
    }
    return output;
}

function shiftMap(deltaX, deltaY){
    var objects = Canvas.getObjects();
    for (var i in objects) {
        objects[i].left = objects[i].left - deltaX;
        objects[i].top = objects[i].top - deltaY;
    }
    Canvas.renderAll();
}