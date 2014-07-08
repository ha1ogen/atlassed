Main = {
    Logo : null,
    Instructions : null,
    Watermark : null,
    SelectBuilding : null,
    SelectFloor : null,
    CenterTiles : null,
    ZoomTiles : null,
    MapLink : null,
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

        Main.SelectBuilding = $('#Buildings');
        Main.SelectFloor = $('#Floors');
        Main.CenterTiles = $('#CenterTiles');
        Main.ZoomTiles = $('#ZoomTiles');
        Main.MapLink = $('#MapLink');

        Dialog.init();
        initSearch();
        Login.init();

        Main.SelectBuilding.change(function (evt) {
            if (isNaN($(this).val()))
                return;

            CurrentContext.CurrentBuildingId = $(this).val();
            $(this).find('.' + Main.NullOptionClass).remove();
            Main.GoToBuilding(CurrentContext.CurrentBuildingId);
        });

        Main.SelectFloor.change(function (evt) {
            if (isNaN($(this).val()) || Main.LOADING_FLOORS)
                return;

            $(this).find('.' + Main.NullOptionClass).remove();
            Main.GoToFloor($(this).val());
        });

        Main.CenterTiles.close = function () {
            if (Main.CenterTiles.is(':visible')) {
                Main.CenterTiles.hide('fade');
            } else {
                Main.CenterTiles.hide();
            }
        };
        Main.CenterTiles.open = function () {
            Main.CenterTiles.css({
                left: centerX(Main.CenterTiles),
                top: centerY(Main.CenterTiles)
            });
            if (!Main.CenterTiles.is(':visible')) {
                Main.CenterTiles.show('fade');
            }
        };
        
        // END INITIATE ELEMENT HANDLES

        // ATTACH EVENT HANDLERS
        $('.tile-select').select2({
            containerCssClass: 'tile-select',
            dropdownCssClass: 'tile-select2-dropdown'
        });

        Main.LoadBuildings();

        // GOOGLE MAP
        Main.GoogleMapContainer.open = function () {
            Main.GoogleMapFrame.attr('src', 'http://maps.google.com/?output=embed&q=' + CurrentContext.GetBuilding(CurrentContext.CurrentBuildingId).BuildingAddress);
            Main.GoogleMapContainer.show('fade');
            Main.MAP_SHOWING = true;
        }
        Main.GoogleMapContainer.close = function () {
            Main.GoogleMapContainer.hide('fade');
            Main.MAP_SHOWING = false;
        }

        Main.MapLink.click(Main.GoogleMapContainer.open);

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

        Main.CenterTiles.css('max-width', window.innerWidth - 20);

        if (Main.HOME_PAGE) {
            CenterSearch();
        }
    },
    LoadBuildings : function () {
        var data = CurrentContext.GetAllBuildings();

        Main.SelectBuilding.empty();

        var option = $('<option/>');
        option.text('Building...');
        option.addClass(Main.NullOptionClass);
        Main.SelectBuilding.append(option);

        $.each(data, function (i, e) {
            option = $('<option/>');
            option.val(e.BuildingId);
            option.text(e.BuildingName);
            Main.SelectBuilding.append(option);
        });

        Main.SelectBuilding.change();
    },
    TransformToHomepage : function () {
        Main.Toolbar.MouseModes.hide();
        Main.SelectFloor.hide();
        Main.ZoomTiles.hide();
        Main.MapLink.hide();

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

            Main.MapLink.show('fade');
            Main.SelectFloor.show('fade');

            $('.homepage').removeClass('homepage');

            Main.ResizeElements();
        };

        SearchInput.bind('input', transformFromHomepage);
        Main.SelectFloor.change(transformFromHomepage);
    },
    FormatSearchResults : function (results) {

        CurrentContext.setSearchResults(results);


        SearchResultList.empty();

        if (results.length > 0) {
            $.each(results, function (i, e) {
                Main.AppendSearchResult(e);
            });
        }

        SearchResultSummary.setMatches(results.length);
        SearchResultSummary.open();
        SearchResultContainer.switchPane('list', SearchResultContainer.open);

        $('.search-result').click(function (e) {
            var self = this;
            clickResult.call(this, e, function () {
                var result = $(self).closest('.search-result'),
                    id = result.data('result-id'),
                    type = result.data('result-type');

                Main.ShowDetails(type, id);
            });
            return false;
        });

        function clickResult(e, callback) {
            var innerCallback = function (success) {
                if (callback != undefined) {
                    callback(success);
                }
            };
            var result = $(this).closest('.search-result'),
                id = result.data('result-id'),
                type = result.data('result-type'),
                secondaryId = result.data('result-secondaryid');

            switch (type) {
                case 'Building':
                    Main.GoToBuilding(id);
                    innerCallback(true);
                    break;
                case 'Person':
                    if (secondaryId == 0) return;
                case 'Space':
                case 'Workstation':
                    Main.GoToFloor(secondaryId, function (success) {
                        if (success) {
                            selectObjectByLocationId(id);
                        }
                        innerCallback(success);
                    });
                    break;
            }
        }

        $('.search-result .remove').click(function (e) {
            var self = this;
            clickResult.call(this, e, function () {
                var result = $(self).closest('.data'),
                    id = result.data('result-id'),
                    type = result.data('result-type');

                Main.RemoveEntity(type, id);
            });

            return false;
        });

        $('.search-result .edit').click(function (e) {
            var self = this;
            clickResult.call(this, e, function () {
                var result = $(self).closest('.search-result'),
                    id = result.data('result-id'),
                    type = result.data('result-type');

                Main.EditEntity(type, id);
            });

            return false;
        });
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
    AppendSearchResult : function (resultData) {
        var result = SearchResultTemplate.clone();
        // reset template
        result.removeAttr('id');
        result.css('display', '');
        // properties
        // result.attr('data-result-id', resultData.PrimaryId);
        // result.attr('data-result-type', resultData.Type);
        // result.attr('data-result-secondaryid', resultData.SecondaryId);
        if (resultData.Point !== undefined) {
            result.attr('data-result-point', resultData.Point.X + ',' + resultData.Point.Y);
        }
        // set visible elements
        result.find('.primary-text').text(resultData.PrimaryText);
        result.find('.secondary-text').text(resultData.SecondaryText);
        // result.find('.right').text(resultData.right);

        SearchResultList.append(result);
    },
    GoToBuilding : function (buildingId) {
        Main.LOADING_FLOORS = true;

        $(this).find('.' + Main.NullOptionClass).remove();

        var floors = CurrentContext.GetFloors(buildingId);

        Main.SelectFloor.empty();
        Main.CenterTiles.empty();

        var option = $('<option/>');
        option.text('Floor...');
        option.addClass(Main.NullOptionClass);
        Main.SelectFloor.append(option);

        $.each(floors, function (i, e) {
            // add option to floor selector
            var label = 'Floor ' + e.FloorNumber;

            option = $('<option/>');
            option.val(e.FloorId);
            option.text(label);
            Main.SelectFloor.append(option);

            // add center tile
            Main.CenterTiles.append(CreateTileLink(label, function () {
                Main.SelectFloor.val(e.FloorId).change();
            }, 'large'));
        });

        Main.SelectFloor.change();
        Main.SelectFloor.removeClass('disabled').removeProp('disabled');

        Main.MapDiv.hide('fade');
        Main.Toolbar.MouseModes.hide('fade');
        Main.ZoomTiles.hide('fade');

        if (floors.length == 1) {
            var id = floors[0].FloorId;
            Main.SelectFloor.val(id).change();
            Main.GoToFloor(id);
        }
        else {
            Main.Watermark.show('fade');
            Main.CenterTiles.open();
        }

        Main.LOADING_FLOORS = false;
    },
    GoToFloor : function (floorId, callback) {
        Main.MapDiv.hide('fade', 200, function () {
            CurrentContext.LoadFloor(floorId, function (success) {
                if (success) {
                    Main.CenterTiles.close();
                    Main.Watermark.hide('fade');
                    Main.Toolbar.MouseModes.show('fade');
                    Main.ZoomTiles.show('fade');
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
