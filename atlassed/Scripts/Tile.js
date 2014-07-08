Tile = {
	SelectBuilding : null,
    SelectFloor : null,
    CenterTiles : null,
    ZoomTiles : null,
    MapLink : null,
    initTiles : function () {
	    SelectBuilding = $('#Buildings');
		SelectFloor = $('#Floors');
		CenterTiles = $('#CenterTiles');
		ZoomTiles = $('#ZoomTiles');
		MapLink = $('#MapLink');

		SelectBuilding.change(function (evt) {
	        if (isNaN($(this).val()))
	            return;

	        CurrentContext.CurrentBuildingId = $(this).val();
	        $(this).find('.' + NullOptionClass).remove();
	        Main.GoToBuilding(CurrentContext.CurrentBuildingId);
	    });

	    SelectFloor.change(function (evt) {
	        if (isNaN($(this).val()) || LOADING_FLOORS)
	            return;

	        $(this).find('.' + NullOptionClass).remove();
	        Main.GoToFloor($(this).val());
	    });

	    CenterTiles.close = function () {
	        if (CenterTiles.is(':visible')) {
	            CenterTiles.hide('fade');
	        } else {
	            CenterTiles.hide();
	        }
	    };
	    CenterTiles.open = function () {
	        CenterTiles.css({
	            left: centerX(CenterTiles),
	            top: centerY(CenterTiles)
	        });
	        if (!CenterTiles.is(':visible')) {
	            CenterTiles.show('fade');
	        }
    	};
	}
};


