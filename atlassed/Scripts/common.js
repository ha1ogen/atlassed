function CreateTileLink(label, onclick, size) {
    var tile = $('<div/>'),
        inner = tile.clone();
    tile.addClass('tile-link' + (size == '' ? '' : '-' + size));
    tile.click(onclick);

    inner.text(label);
    return tile.append(inner);
}

function centerX(element) {
    return window.innerWidth / 2 - $(element).width() / 2
}

function centerY(element) {
    return window.innerHeight / 2 - $(element).height() / 2
}

function selectOption(value, label) {
    return $('<option value="' + value + '">' + label + '</option>');
}

function tryCall(callback, arg1, arg2, arg3, arg4) {
    if (typeof callback === 'function') {
        callback.call(null, arg1, arg2, arg3, arg4);
    }
}

function rectGetCentre(topLeft, bottomRight) {
    return {
        X: topLeft.X + (bottomRight.X - topLeft.X) / 2,
        Y: topLeft.Y + (bottomRight.Y - topLeft.Y) / 2
    }
}
