Tile = {
	SelectBuilding : null,
    SelectFloor : null,
    CenterTiles : null,
    ZoomTiles : null,
    MapLink : null,
    init : function () {
	    Tile.SelectBuilding = $('#Buildings');
		Tile.SelectFloor = $('#Floors');
		Tile.CenterTiles = $('#CenterTiles');
		Tile.ZoomTiles = $('#ZoomTiles');
		Tile.MapLink = $('#MapLink');

		Tile.SelectBuilding.change(function (evt) {
	        if (isNaN($(this).val()))
	            return;

	        CurrentContext.CurrentBuildingId = $(this).val();
	        $(this).find('.' + Main.NullOptionClass).remove();
	        Main.GoToBuilding(CurrentContext.CurrentBuildingId);
	    });

	    Tile.SelectFloor.change(function (evt) {
	        if (isNaN($(this).val()) || Main.LOADING_FLOORS)
	            return;

	        $(this).find('.' + Main.NullOptionClass).remove();
	        Main.GoToFloor($(this).val());
	    });

	    Tile.CenterTiles.close = function () {
	        if (Tile.CenterTiles.is(':visible')) {
	            Tile.CenterTiles.hide('fade');
	        } else {
	            Tile.CenterTiles.hide();
	        }
	    };
	    Tile.CenterTiles.open = function () {
	        Tile.CenterTiles.css({
	            left: centerX(Tile.CenterTiles),
	            top: centerY(Tile.CenterTiles)
	        });
	        if (!Tile.CenterTiles.is(':visible')) {
	            Tile.CenterTiles.show('fade');
	        }
    	};
	}
};


