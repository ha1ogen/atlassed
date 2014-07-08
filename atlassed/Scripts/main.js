Main = {
    Logo : null,
    Instructions : null,
    Watermark : null,
    GoogleMapContainer : null,
    GoogleMapFrame : null,
    MapDiv : null,
    Pin : null,
    Toolbar : null,
    MouseModesGroup : null,
    NullOptionClass : 'null',
    HOME_PAGE : true,
    LOADING_FLOORS : false,
    MAP_SHOWING : false,

    init : function () {
        Main.Logo = $('#Logo');
        Main.Instructions = $('#Instructions');
        Main.Watermark = $('#Watermark');
        
        Main.GoogleMapContainer = $('#GoogleMapContainer');
        Main.GoogleMapFrame = $('#GoogleMapFrame');
        Main.MapDiv = $('#canvasWrapper');

        Main.Toolbar = $('#Toolbar');
        Main.Toolbar.MouseModes = Main.Toolbar.find('#MouseModes');
        Main.Toolbar.DataActions = Main.Toolbar.find('#DataActions');

        Dialog.init();
        initSearch();
        Login.init();
        Tile.init();
        
        // END INITIATE ELEMENT HANDLES

        // ATTACH EVENT HANDLERS
        $('.tile-select').select2({
            containerCssClass: 'tile-select',
            dropdownCssClass: 'tile-select2-dropdown'
        });

        Main.LoadBuildings();

        // GOOGLE MAP
        Main.GoogleMapContainer.open = function () {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(Position);
                function Position(position){
                    Main.GoogleMapFrame.attr('src', 'https://www.google.com/maps/embed/v1/directions?key=AIzaSyCpJLHAqXrv-p8a38SWDdd5VYuASFdrqR4&destination=' + CurrentContext.GetBuilding(CurrentContext.CurrentBuildingId).BuildingAddress + '&origin=' + position.coords.latitude + ',' + position.coords.longitude);
                }
            } else { 
                //Geolocation not supported by browser, use normal one point position
                Main.GoogleMapFrame.attr('src', 'https://www.google.com/maps/embed/v1/place?key=AIzaSyCpJLHAqXrv-p8a38SWDdd5VYuASFdrqR4&q=' + CurrentContext.GetBuilding(CurrentContext.CurrentBuildingId).BuildingAddress);
            }
            Main.GoogleMapContainer.show('fade');
            Main.MAP_SHOWING = true;
        }
        Main.GoogleMapContainer.close = function () {
            Main.GoogleMapContainer.hide('fade');
            Main.MAP_SHOWING = false;
        }

        $(document).keydown(function (e) {
            // ESC MAP
            if (e.which === 27 && Main.MAP_SHOWING) {
                Main.GoogleMapContainer.close();
            }
            else if (e.which === 13 && $('#LoginWrapper').is(':visible')) {
                Login.login();
            }

            if (SearchInput.is(':focus')) {
                switch (e.which) {
                    case 13:
                        $('.search-result')[0].click();
                        break;
                    default:
                }
            }
        });

        Main.GoogleMapContainer.click(Main.GoogleMapContainer.close);
        // END GOOGLE MAP

        

        // Main.Toolbar BUTTONS
        Main.Toolbar.MouseModes.find('.toolbar-button').click(function () {
            CurrentContext.CurrentTool($(this).attr('id'));
            Main.Toolbar.MouseModes.find('.selected').removeClass('selected');
            $(this).addClass('selected');
        });
        $('#ZoomIn').click(function () {
            Scrolling = +1;
            zoom(+2);
        });
        $('#ZoomOut').click(function () {
            Scrolling = -1;
            zoom(-2);
        });
        
        Main.Toolbar.find('#T_BUILDINGS').click(function () {
            BuildingDialog.open();
            PersonDialog.close();
            return false;
        });
        Main.Toolbar.find('#T_PEOPLE').click(function () {
            PersonDialog.open();
            BuildingDialog.close();
            return false;
        });

        Main.Toolbar.find('.toolbar-button').click(function () {
            return false;
        });

        $(window).resize(Main.ResizeElements);
        // END ATTACH EVENT HANDLERS

        // TRANSFORM TO HOMEPAGE
        Main.TransformToHomepage();

        Main.ResizeElements(); // initial sizing
        if (!CurrentContext.IsAdmin()) {
            $('.admin-only').hide();
        }
    },
    ShowDetails : function (type, id, callback) {
        var title = "",
            details = {},
            obj;
        switch (type) {
            case 'Building':
                var b = CurrentContext.GetBuilding(id);
                title = b.BuildingName;
                details = {
                    "": b.BuildingAddress
                };
                break;
            case 'Space':
                obj = getObjectByLocationId(id);
                var s = obj.w;
                title = s.Name;
                details = {
                    Number: s.Number,
                    Location: s.Floor.Description
                };
                PlacePin(obj);
                break;
            case 'Workstation':
                obj = getObjectByLocationId(id);
                var w = obj.locationObj;
                title = "Location "; //+ w.Number;
                details = {
                    Name: w.Name,
                    Description: w.Description,
                    Type: w.Type == null ? "Undefined" : w.Type,
                    Assigned: w.AssignedPerson == null ? "Unassigned" : w.AssignedPerson.Name
                };
                PlacePin(obj);
                break;
            case 'Person':
                var p = CurrentContext.GetPerson(id);
                title = p.Name;
                details = {
                    Team: p.Description
                };
                PlacePin(getObjectByLocationId(p.LocationId));
                break;
            default:
        }
        SearchResultsDetailsCard.fillAndShow(type, id, title, details, callback);
    },
    ResizeCanvasWrapper : function () {
        if (!CanvasLoaded()) {
            return;
        }

        // resizes the wrapper to it can be centered horizontally...
        Main.MapDiv.width(Math.min(window.innerWidth - 10, Canvas.getWidth()));

        // ...and vertically
        Main.MapDiv.height(Math.min(window.innerHeight - 10, Canvas.getHeight()));
        var top = window.innerHeight - Main.MapDiv.height();
        Main.MapDiv.css({
            marginTop: top / 2
        });
    },
    ResizeElements : function () {
        SearchResultContainer.css({
            maxHeight: window.innerHeight - 150
        });
        SearchResultList.css({
            maxHeight: window.innerHeight - 150
        });

        Main.Watermark.css({
            left: centerX(Main.Watermark),
            top: centerY(Main.Watermark)
        });

        Main.GoogleMapContainer
            .height(window.innerHeight)
            .width(window.innerWidth);

        Main.Toolbar.css('left', SearchContainer.width());

        Main.ResizeCanvasWrapper();

        Tile.CenterTiles.css('max-width', window.innerWidth - 20);

        if (Main.HOME_PAGE) {
            CenterSearch();
        }
    },
    LoadBuildings : function () {
        var data = CurrentContext.GetAllBuildings();

        Tile.SelectBuilding.empty();

        var option = $('<option/>');
        option.text('Building...');
        option.addClass(Main.NullOptionClass);
        Tile.SelectBuilding.append(option);

        $.each(data, function (i, e) {
            option = $('<option/>');
            option.val(e.BuildingId);
            option.text(e.BuildingName);
            Tile.SelectBuilding.append(option);
        });

        Tile.SelectBuilding.change();
    },
    TransformToHomepage : function () {
        Main.Toolbar.MouseModes.hide();
        Tile.SelectFloor.hide();
        Tile.ZoomTiles.hide();
        Tile.MapLink.hide();

        var searchLeft = SearchContainer.css('left'),
            searchTop = SearchContainer.css('top');

        CenterSearch();

        function transformFromHomepage() {
            if (!Main.HOME_PAGE)
                return;

            Main.HOME_PAGE = false;

            Main.Logo.hide();
            Main.Instructions.hide();
            SearchContainer.animate({
                left: searchLeft,
                top: searchTop
            });

            Tile.MapLink.show('fade');
            Tile.SelectFloor.show('fade');

            $('.homepage').removeClass('homepage');

            Main.ResizeElements();
        };

        SearchInput.bind('input', transformFromHomepage);
        Tile.SelectFloor.change(transformFromHomepage);
    },
    EditEntity: function (type, id) {
        switch (type) {
            case 'Building':
                BuildingDialog.open(id);
                break;
            case 'Space':
                var s = getObjectByLocationId(id);
                SpaceDialog.open(id, s, s.TopLeft, s.BottomRight, function (newObject) {
                    if (newObject != null) {
                        s.w = newObject;
                    }
                });
                break;
            case 'Workstation':
                var w = getObjectByLocationId(id);
                WorkstationDialog.open(id, w, w.Point, function (newObject) {
                    if (newObject != null) {
                        w.w = newObject;
                    }
                });
                break;
            case 'Person':
                PersonDialog.open(id);
                break;
            default:
                throw 'invalid type';
        }
    },
    RemoveEntity : function (type, id) {

        switch (type) {
            case 'Building':
                if (!confirm('Are you sure you want to remove this ' + type.toLowerCase() + '?')) {
                    return;
                }
                CurrentContext.RemoveBuilding(id);
                break;
            case 'Space':
                removeSpace(getObjectByLocationId(id));
                CurrentContext.RemoveSpace(id);
                break;
            case 'Workstation':
                removeWorkstation(getObjectByLocationId(id));
                break;
            case 'Person':
                if (!confirm('Are you sure you want to remove this ' + type.toLowerCase() + '?')) {
                    return;
                }
                CurrentContext.RemovePerson(id);
                break;
            default:
                throw 'invalid type';
        }
    },
    GoToBuilding : function (buildingId) {
        Main.LOADING_FLOORS = true;

        $(this).find('.' + Main.NullOptionClass).remove();

        var floors = CurrentContext.GetFloors(buildingId);

        Tile.SelectFloor.empty();
        Tile.CenterTiles.empty();

        var option = $('<option/>');
        option.text('Floor...');
        option.addClass(Main.NullOptionClass);
        Tile.SelectFloor.append(option);

        $.each(floors, function (i, e) {
            // add option to floor selector
            var label = 'Floor ' + e.FloorNumber;

            option = $('<option/>');
            option.val(e.FloorId);
            option.text(label);
            Tile.SelectFloor.append(option);

            // add center tile
            Tile.CenterTiles.append(CreateTileLink(label, function () {
                Tile.SelectFloor.val(e.FloorId).change();
            }, 'large'));
        });

        Tile.SelectFloor.change();
        Tile.SelectFloor.removeClass('disabled').removeProp('disabled');

        Main.MapDiv.hide('fade');
        Main.Toolbar.MouseModes.hide('fade');
        Tile.ZoomTiles.hide('fade');

        if (floors.length == 1) {
            var id = floors[0].FloorId;
            Tile.SelectFloor.val(id).change();
            Main.GoToFloor(id);
        }
        else {
            Main.Watermark.show('fade');
            Tile.CenterTiles.open();
        }

        Main.LOADING_FLOORS = false;
    },
    GoToFloor : function (floorId, callback) {
        Main.MapDiv.hide('fade', 200, function () {
            CurrentContext.LoadFloor(floorId, function (success) {
                if (success) {
                    Tile.CenterTiles.close();
                    Main.Watermark.hide('fade');
                    Main.Toolbar.MouseModes.show('fade');
                    Tile.ZoomTiles.show('fade');
                    Main.MapDiv.show('fade', 200);
                    Main.ResizeElements();
                } else {
                    alert('Error loading floorplan');
                }
                if (callback != undefined) {
                    callback(success);
                }
            });
        });
    }
}



$().ready(function () {
    Main.init();
    
});
