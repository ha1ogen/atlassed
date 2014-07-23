var VALID_IMG_TYPE = 'image/png';

$().ready(function () {
    // call initialization file
    if (window.File && window.FileList && window.FileReader) {
        Init();
    }
});

//
// initialize
function Init() {
    var fileselect = $("#fileselect"),
		filedrag = $("#filedrag");

    // file select
    fileselect.change(FileSelectHandler);

    // is XHR2 available?
    var xhr = new XMLHttpRequest();
    if (xhr.upload) {

        // file drop
        filedrag.on('dragover', FileDragHover);
        filedrag.on('dragleave', FileDragHover);
        filedrag.on('drop', FileSelectHandler);
        filedrag.css('display', 'block');
    }
}

// file drag hover
function FileDragHover(e) {
    e.stopPropagation();
    e.preventDefault();
    e.target.className = (e.type == "dragover" ? "hover" : "");
}

// file selection
function FileSelectHandler(e) {
    // cancel event and hover styling
    FileDragHover(e);

    // fetch FileList object
    var files = e.target.files || e.originalEvent.dataTransfer.files;

    // process all File objects
    PreviewFile(files[0]);
    UploadFile(files[0], 'http://localhost:81/api/upload/images/map/1145'); // need to determine URI and id
}

function PreviewFile(file) {    
    if (file.type === VALID_IMG_TYPE) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('img#preview').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    }
}

function UploadFile(file, uri) {

    var xhr = new XMLHttpRequest();
    if (xhr.upload && file.type == VALID_IMG_TYPE)
    {
        // start upload
        xhr.open('post', uri, true);
        xhr.setRequestHeader('X_FILENAME', file.name);
        // add Session-Id header...
        xhr.send(file);
    }
}