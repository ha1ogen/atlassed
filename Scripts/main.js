var Logo,
    Instructions,
    Watermark,
    SearchContainer,
    SearchInput,
    SearchResultContainer,
    SearchResultSummary,
    SearchResultsDetailsCard,
    SearchResultList,
    SearchResultTemplate,
    SearchResultDetail,
    SelectBuilding,
    SelectFloor,
    CenterTiles,
    ZoomTiles,
    MapLink,
    GoogleMapContainer,
    GoogleMapFrame,
    MapDiv,
    Pin,
    Toolbar,
    MouseModesGroup;

var BuildingDialog,
    SpaceDialog,
    WorkstationDialog,
    PersonDialog;

var NullOptionClass = 'null';

var HOME_PAGE = true,
    LOADING_FLOORS = false,
    MAP_SHOWING = false,
    SEARCH_RESULT_CLOSING = false;

$().ready(function () {
    // INITIATE ELEMENT HANDLES
    Logo = $('#Logo');
    Instructions = $('#Instructions');
    Watermark = $('#Watermark');
    SearchContainer = $('#Search');
    SearchInput = $('#SearchBox input');
    SearchResultContainer = $('#SearchResults');
    SearchResultSummary = $('#SearchResultSummary');
    SearchResultsDetailsCard = $('#DetailsCard');
    SearchResultList = $('#SearchResultList');
    SearchResultTemplate = $('#SearchResultTemplate');
    SearchResultDetail = $('#SearchResultDetail');
    SelectBuilding = $('#Buildings');
    SelectFloor = $('#Floors');
    CenterTiles = $('#CenterTiles');
    ZoomTiles = $('#ZoomTiles');
    MapLink = $('#MapLink');
    GoogleMapContainer = $('#GoogleMapContainer');
    GoogleMapFrame = $('#GoogleMapFrame');
    MapDiv = $('#canvasWrapper');

    Toolbar = $('#Toolbar');
    Toolbar.MouseModes = Toolbar.find('#MouseModes');
    Toolbar.DataActions = Toolbar.find('#DataActions');

    InitializeDialogs();

    SearchContainer.closeAll = function () {
        SearchResultSummary.close();
        SearchResultContainer.close();
    };
    SearchContainer.openAll = function () {
        SearchResultSummary.open();
        SearchResultContainer.open();
    };
    
    SearchResultContainer.currentPane = 'list';
    SearchResultContainer.open = function (callback) {
        if (!SearchResultContainer.is(':visible') && 
            !(SearchResultContainer.currentPane == 'list' && 
                SearchResultList.isEmpty())) {
            SearchResultContainer.show('slide', { direction: 'up' }, 200, callback);
        } else {
            tryCall(callback);
        }
    };
    SearchResultContainer.close = function (callback) {
        if (SearchResultContainer.is(':visible')) {
            SearchResultContainer.hide('slide', { direction: 'up' }, 200, callback);
        } else {
            SearchResultContainer.hide();
            tryCall(callback);
        }
    };
    SearchResultContainer.switchPane = function (toPane, callback, detailId) {
        SearchResultContainer.open();
        if (toPane == 'detail' && SearchResultContainer.currentPane != 'detail') {
            SearchResultContainer.currentPane = 'detail';
            if (detailId != SearchResultsDetailsCard.currentId) {
                SearchResultsDetailsCard.currentId = detailId;
                SearchResultsDetailsCard.show('slide', 
                    { direction: SearchResultList.is(':visible') ? 'right' : 'up' }, 200);
                SearchResultList.close('left');
            } else {
                tryCall(callback);
            }
        } else if (toPane == 'list' && SearchResultContainer.currentPane != 'list') {
            SearchResultContainer.currentPane = 'list';
            SearchResultsDetailsCard.currentId = 0;
            SearchResultsDetailsCard.close('right');
            SearchResultList.open('left', callback);
        }
    };

    
    SearchResultSummary.open = function (callback) {
        if (!SearchResultSummary.is(':visible')) {
            SearchResultSummary.show('slide', { direction: 'up' }, 200, callback);
        } else {
            tryCall(callback);
        }
    };
    SearchResultSummary.close = function (callback) {
        if (SearchResultSummary.is(':visible')) {
            SearchResultSummary.hide('slide', { direction: 'up' }, 200, callback);
        } else {
            SearchResultSummary.hide();
            tryCall(callback);
        }
    };
    SearchResultSummary.setMatches = function (numMatches) {
        SearchResultSummary.find('span').text(numMatches + ' Match' + (numMatches === 1 ? '' : 'es'));
    }

    
    SearchResultsDetailsCard.currentId = 0;
    SearchResultsDetailsCard.fillAndShow = function (type, id, title, details, callback) {
        // toggle "Back to Results" link
        SearchResultList.isEmpty() ? $('#BackToResults').hide() : $('#BackToResults').show();

        $('#DetailsCardTitle').text(title);
        // fill details table
        var table = $('#DetailsCardTable');
        table.empty();
        $.each(details || [], function (label, value) {
            table.append('<tr><td>' + label + '</td><td>' + value + '</td></tr>');
        });
        SearchResultsDetailsCard.find('.remove').off().click(function () {
            RemoveEntity(type, id);
        });
        SearchResultsDetailsCard.find('.edit').off().click(function () {
            EditEntity(type, id);
        });

        SearchResultContainer.switchPane('detail', callback, id);
    };
    SearchResultsDetailsCard.close = function (direction, callback) {
        if (SearchResultsDetailsCard.is(':visible')) {
            SearchResultsDetailsCard.hide('slide', { direction: direction }, 200, callback);
            SearchResultContainer.close();
        } else {
            SearchResultsDetailsCard.hide();
            tryCall(callback);
        }
    }
    SearchResultsDetailsCard.clear = function () {
        $('#DetailsCardTitle').text('');
        // fill details table
        var table = $('#DetailsCardTable');
        table.empty();

        SearchResultsDetailsCard.close('up');
    }

    
    SearchResultList.open = function (direction, callback) {
        if (!SearchResultList.is(':visible')) {
            SearchResultList.show('slide', { direction: direction }, 200, callback);
        } else {
            tryCall(callback);
        }
    };
    SearchResultList.close = function (direction, callback) {
        if (SearchResultList.is(':visible')) {
            SearchResultList.hide('slide', { direction: direction }, 200, callback);
        } else {
            SearchResultList.hide();
            tryCall(callback);
        }
    };
    SearchResultList.isEmpty = function () {
        return CurrentContext.setSearchResults().length == 0;
    };

    
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
    // END INITIATE ELEMENT HANDLES

    // ATTACH EVENT HANDLERS
    $('.tile-select').select2({
        containerCssClass: 'tile-select',
        dropdownCssClass: 'tile-select2-dropdown'
    });

    LoadBuildings();

    SelectBuilding.change(function (evt) {
        if (isNaN($(this).val()))
            return;

        CurrentContext.CurrentBuildingId = $(this).val();
        $(this).find('.' + NullOptionClass).remove();
        GoToBuilding(CurrentContext.CurrentBuildingId);
    });

    SelectFloor.change(function (evt) {
        if (isNaN($(this).val()) || LOADING_FLOORS)
            return;

        $(this).find('.' + NullOptionClass).remove();
        GoToFloor($(this).val());
    });

    // GOOGLE MAP
    GoogleMapContainer.open = function () {
        GoogleMapFrame.attr('src', 'http://maps.google.com/?output=embed&q=' + CurrentContext.GetBuilding(CurrentContext.CurrentBuildingId).BuildingAddress);
        GoogleMapContainer.show('fade');
        MAP_SHOWING = true;
    }
    GoogleMapContainer.close = function () {
        GoogleMapContainer.hide('fade');
        MAP_SHOWING = false;
    }

    MapLink.click(GoogleMapContainer.open);

    $(document).keydown(function (e) {
        // ESC MAP
        if (e.which = 27 && MAP_SHOWING) {
            GoogleMapContainer.close();
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

    GoogleMapContainer.click(GoogleMapContainer.close);
    // END GOOGLE MAP

    SearchInput
        .bind('input', function (evt) {
            var query = $(this).val();
            SearchResultContainer.switchPane('list');

            if (query.length === 0) {
                CurrentContext.ClearSearch();

                SearchContainer.closeAll();
                return;
            }

            var results = CurrentContext.search(query);
            FormatSearchResults(results);
            // ajax({
            //     webservice: 'Main',
            //     func: 'Search',
            //     params: { query: query },
            //     success: FormatSearchResults
            // });
        })
        .focus(function () {
            OnFocus_SearchArea();
        })
        .blur(function (e) {
            SearchContainer.closeAll();
        });

    $('#ClearSearch').click(function () {
        SearchResultSummary.setMatches(0);
        SearchResultList.empty();
        SearchContainer.closeAll();
        CurrentContext.ClearSearch();
        SearchInput.val('');
    });

    SearchResultsDetailsCard.find('#BackToResults').click(function () {
        SearchResultContainer.switchPane('list');
    });

    // TOOLBAR BUTTONS
    Toolbar.MouseModes.find('.toolbar-button').click(function () {
        CurrentContext.CurrentTool($(this).attr('id'));
        Toolbar.MouseModes.find('.selected').removeClass('selected');
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
    $('#login').click(function () {
        $('#LoginWrapper').css('visibility','hidden');
        $('#AppWrapper').css('visibility','visible');
        if ($('#username').val() == 'admin'){
            admin = true;
        }
        else{
            admin = false;
            $('#Toolbar').css('visibility','hidden');
        }
    });

    Toolbar.find('#T_BUILDINGS').click(function () {
        BuildingDialog.open();
        PersonDialog.close();
        return false;
    });
    Toolbar.find('#T_PEOPLE').click(function () {
        PersonDialog.open();
        BuildingDialog.close();
        return false;
    });

    Toolbar.find('.toolbar-button').click(function () {
        return false;
    });

    $(window).resize(ResizeElements);
    // END ATTACH EVENT HANDLERS

    // TRANSFORM TO HOMEPAGE
    TransformToHomepage();

    ResizeElements(); // initial sizing
    if (!CurrentContext.IsAdmin()) {
        $('.admin-only').hide();
    }
});

function OnFocus_SearchArea() {
    if (CurrentContext.setSearchResults().length > 0) {
        SearchContainer.openAll();
    }
}

function OnBlur_SearchArea() {
    SearchContainer.closeAll();
}

function ShowDetails(type, id, callback) {
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
            title = "Workstation " + w.Number;
            details = {
                Port: w.PortNumber,
                Location: w.Description,
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
}

function ResizeCanvasWrapper() {
    if (!CanvasLoaded()) {
        return;
    }

    // resizes the wrapper to it can be centered horizontally...
    MapDiv.width(Math.min(window.innerWidth - 10, Canvas.getWidth()));

    // ...and vertically
    MapDiv.height(Math.min(window.innerHeight - 10, Canvas.getHeight()));
    var top = window.innerHeight - MapDiv.height();
    MapDiv.css({
        marginTop: top / 2
    });
}

function ResizeElements() {
    SearchResultContainer.css({
        maxHeight: window.innerHeight - 150
    });
    SearchResultList.css({
        maxHeight: window.innerHeight - 150
    });

    Watermark.css({
        left: centerX(Watermark),
        top: centerY(Watermark)
    });

    GoogleMapContainer
        .height(window.innerHeight)
        .width(window.innerWidth);

    Toolbar.css('left', SearchContainer.width());

    ResizeCanvasWrapper();

    CenterTiles.css('max-width', window.innerWidth - 20);

    if (HOME_PAGE) {
        CenterSearch();
    }
}

function LoadBuildings() {
    var data = CurrentContext.GetAllBuildings();

    SelectBuilding.empty();

    var option = $('<option/>');
    option.text('Building...');
    option.addClass(NullOptionClass);
    SelectBuilding.append(option);

    $.each(data, function (i, e) {
        option = $('<option/>');
        option.val(e.BuildingId);
        option.text(e.BuildingName);
        SelectBuilding.append(option);
    });

    SelectBuilding.change();
}

function TransformToHomepage() {
    Toolbar.MouseModes.hide();
    SelectFloor.hide();
    ZoomTiles.hide();
    MapLink.hide();

    var searchLeft = SearchContainer.css('left'),
        searchTop = SearchContainer.css('top');

    CenterSearch();

    function transformFromHomepage() {
        if (!HOME_PAGE)
            return;

        HOME_PAGE = false;

        Logo.hide();
        Instructions.hide();
        SearchContainer.animate({
            left: searchLeft,
            top: searchTop
        });

        MapLink.show('fade');
        SelectFloor.show('fade');

        $('.homepage').removeClass('homepage');

        ResizeElements();
    };

    SearchInput.bind('input', transformFromHomepage);
    SelectFloor.change(transformFromHomepage);
}

function CenterSearch() {
    SearchContainer
        .css({
            left: centerX(SearchContainer),
            top: centerY(SearchContainer) - 100
        });
}

function FormatSearchResults(results) {

    CurrentContext.setSearchResults(results);


    SearchResultList.empty();

    if (results.length > 0) {
        $.each(results, function (i, e) {
            AppendSearchResult(e);
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

            ShowDetails(type, id);
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
                GoToBuilding(id);
                innerCallback(true);
                break;
            case 'Person':
                if (secondaryId == 0) return;
            case 'Space':
            case 'Workstation':
                GoToFloor(secondaryId, function (success) {
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

            RemoveEntity(type, id);
        });

        return false;
    });

    $('.search-result .edit').click(function (e) {
        var self = this;
        clickResult.call(this, e, function () {
            var result = $(self).closest('.search-result'),
                id = result.data('result-id'),
                type = result.data('result-type');

            EditEntity(type, id);
        });

        return false;
    });
}

function EditEntity(type, id) {
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
}

function RemoveEntity(type, id) {

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
}

function AppendSearchResult(resultData) {
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
}

function GoToBuilding(buildingId) {
    LOADING_FLOORS = true;

    $(this).find('.' + NullOptionClass).remove();

    var floors = CurrentContext.GetFloors(buildingId);

    SelectFloor.empty();
    CenterTiles.empty();

    var option = $('<option/>');
    option.text('Floor...');
    option.addClass(NullOptionClass);
    SelectFloor.append(option);

    $.each(floors, function (i, e) {
        // add option to floor selector
        var label = 'Floor ' + e.FloorNumber;

        option = $('<option/>');
        option.val(e.FloorId);
        option.text(label);
        SelectFloor.append(option);

        // add center tile
        CenterTiles.append(CreateTileLink(label, function () {
            SelectFloor.val(e.FloorId).change();
        }, 'large'));
    });

    SelectFloor.change();
    SelectFloor.removeClass('disabled').removeProp('disabled');

    MapDiv.hide('fade');
    Toolbar.MouseModes.hide('fade');
    ZoomTiles.hide('fade');

    if (floors.length == 1) {
        var id = floors[0].FloorId;
        SelectFloor.val(id).change();
        GoToFloor(id);
    }
    else {
        Watermark.show('fade');
        CenterTiles.open();
    }

    LOADING_FLOORS = false;
}

function GoToFloor(floorId, callback) {
    MapDiv.hide('fade', 200, function () {
        CurrentContext.LoadFloor(floorId, function (success) {
            if (success) {
                CenterTiles.close();
                Watermark.hide('fade');
                Toolbar.MouseModes.show('fade');
                ZoomTiles.show('fade');
                MapDiv.show('fade', 200);
                ResizeElements();
            } else {
                alert('Error loading floorplan');
            }
            if (callback != undefined) {
                callback(success);
            }
        });
    });
}

function InitializeDialogs() {
    BuildingDialog = $('#BuildingDialog');
    SpaceDialog = $('#SpaceDialog');
    WorkstationDialog = $('#WorkstationDialog');
    PersonDialog = $('#PersonDialog');

    // BUILDING DIALOG
    var BuildingInputs = $('#BuildingInputs'),
        FloorList = $('#FloorList'),
        FloorTemplate = FloorList.find('.template');

    BuildingDialog.dialog({
        title: "Buildings",
        autoOpen: false,
        buttons: {
            Cancel: function () {
                BuildingDialog.dialog('close');
            },
            Save: function () {
                var name = BuildingInputs.find('.building-name').val();
                var address = BuildingInputs.find('.building-address').val();
                var floors = [];
                FloorList.find('tbody').children().each(function (i, f) {
                    f = $(f);
                    floors.push({
                        FloorId: f.attr('data-floor-id'),
                        FloorNumber: f.find('.floor-number').val(),
                        Filename: f.find('.floor-filename').val()
                    });
                });

                var validationMessage = '';
                if (name.length == 0) {
                    validationMessage += 'Building name is required. ';
                }
                if (floors.length == 0) {
                    validationMessage += 'Each building must have at least one floor. ';
                }
                if (validationMessage.length > 0) {
                    alert(validationMessage);
                    return;
                }

                if (!CurrentContext.SaveBuilding(name, address, floors)) {
                    alert('Unable to save building!');
                    return;
                }

                BuildingDialog.dialog('close');
            }
        },
        modal: true,
        width: 380,
        position: { my: 'center top', at: 'center top+5' }
    });
    BuildingDialog.open = function (buildingId) {
        if (!CurrentContext.IsAdmin()) {
            return;
        }

        removeAllFloors();
        if (buildingId === undefined) {
            addFloorRow();
            BuildingInputs.find('.building-name').val('');
            BuildingInputs.find('.building-address').val('');
        } else {
            var b = CurrentContext.GetBuilding(buildingId);
            BuildingInputs.find('.building-name').val(b.BuildingName);
            BuildingInputs.find('.building-address').val(b.BuildingAddress);

            $.each(b.Floors, function (j, f) {
                addFloorRow(f.floorId, f.FloorNumber, f.Filename);
            });
        }

        this.dialog('open');
    }
    BuildingDialog.close = function () {
        this.dialog('close');
    }
    BuildingDialog.find('.add-floor').click(function () {
        addFloorRow();
    });

    function addFloorRow(floorId, number, filename) {
        var tr = FloorTemplate.clone();
        tr.removeClass('template');
        tr.attr('data-floor-id', floorId);
        tr.find('.floor-number').val(number);
        tr.find('.floor-filename').val(filename);
        tr.find('.floor-remove').click(function () {
            if (confirm('Are you sure you want to permanently remove this floor?')) {
                tr.remove();
            }
            return false;
        });
        FloorList.find('tbody').append(tr);
    }

    function removeAllFloors() {
        FloorList.find('tbody').empty();
    }

    // SPACE
    SpaceDialog.dialog({
        title: "Add/Edit Room",
        autoOpen: false,
        buttons: {
            Cancel: function () {
                SpaceDialog.dialog('close');
                SpaceDialog.saveCallback(null);
            },
            Save: function () {
                var success = false;

                var number = $('#SpaceNumber').val(),
                    name = $('#SpaceName').val();

                var vMessage = '';
                if (number.length == 0) {
                    vMessage += 'Workspace number is required. ';
                }
                if (name.length == 0) {
                    vMessage += 'Workspace name is required. ';
                }
                if (vMessage.length !== 0) {
                    alert(vMessage);
                } else {
                    success = true;
                }

                var w;

                if (SpaceDialog.locationId == 0) { //(point1, point2, spaceName, spaceNumber)
                    w = CurrentContext.AddSpace(SpaceDialog.topLeftPoint, SpaceDialog.bottomRightPoint, name, number);
                } else { //(locationId, point1, point2, spaceName, spaceNumber) 
                    w = CurrentContext.SaveSpace(SpaceDialog.locationId, name, number);
                }
                if (success) {
                    $(this).dialog('close');
                }
                SpaceDialog.saveCallback(w);
            }
        },
        modal: true,
        position: { my: 'center top', at: 'center top+5' }
    });

    //Pass locationId = 0 if adding
    SpaceDialog.open = function (locationId, w, topLeftPoint, bottomRightPoint, callback) {
        if (!CurrentContext.IsAdmin()) {
            return;
        }

        if (locationId == 0 && w == null) {
            $('#SpaceNumber').val('');
            $('#SpaceName').val('');
        } else {
            $('#SpaceNumber').val(w.w.Number);
            $('#SpaceName').val(w.w.Name);
            locationId = w.w.LocationId;
        }

        SpaceDialog.topLeftPoint = topLeftPoint;
        SpaceDialog.bottomRightPoint = bottomRightPoint;
        SpaceDialog.saveCallback = function (result) { tryCall(callback, result); };
        SpaceDialog.locationId = locationId;

        this.dialog('open');
    };

    // WORKSTATION
    var AssignWorkstation = $('#AssignWorkstation');
    AssignWorkstation.select2();

    WorkstationDialog.saveCallback = undefined;
    WorkstationDialog.dialog({
        title: "Add/Edit Workstation",
        autoOpen: false,
        buttons: {
            Cancel: function () {
                $(this).dialog('close');
                WorkstationDialog.saveCallback(null);
            },
            Save: function () {
                var success = false;

                var number = $('#WorkstationNumber').val(),
                    port = $('#WorkstationPort').val(),
                    assignee = $('#AssignWorkstation').val();

                var vMessage = '';
                if (number.length == 0) {
                    vMessage += 'Workstation number is required';
                }
                if (vMessage.length !== 0) {
                    alert(vMessage);
                } else {
                    success = true;
                }

                var w;

                if (AddingObject) {//point, workspaceId, number, port, personId
                    w = CurrentContext.AddWorkstation(WorkstationDialog.currentPoint, WorkstationDialog.locationId, number, port, assignee);
                } else {//workstationId, point, number, port, personId
                    w = CurrentContext.SaveWorkstation(WorkstationDialog.locationId, number, port, assignee);
                }

                if (w == null) {
                    alert("Error saving workstation");
                    return;
                }

                AddingObject = false;
                if (success) {
                    $(this).dialog('close');
                }
                WorkstationDialog.saveCallback(w);
                setTimeout(function () {
                    ShowDetails(TYPE_WORKSTATION, w.LocationId);
                }, 50);
            }
        },
        modal: true,
        position: { my: 'center top', at: 'center top+5' }
    });
    WorkstationDialog.open = function (parentLocationId, w, point, callback) {
        if (!CurrentContext.IsAdmin()) {
            return;
        }

        AssignWorkstation.empty();
        //AssignWorkstation.append('<option>Unassigned</option>');
        AssignWorkstation.append(selectOption(0, 'Unassigned'));
        CurrentContext.GetUnassignedPeople(function (data) {
            $.each(data, function (i, p) {
                AssignWorkstation.append(selectOption(p.PersonId, p.Name));
            });
        });
        AssignWorkstation.prev().width(200);

        var locationId;
        if (w == null) {
            $('#WorkstationNumber').val('');
            $('#WorkstationPort').val('');
            locationId = parentLocationId;
        } else {
            $('#WorkstationNumber').val(w.w.Number);
            $('#WorkstationPort').val(w.w.PortNumber);
            var person = w.w.AssignedPerson;
            if (person != null) {
                AssignWorkstation.append(selectOption(person.PersonId, person.Name)).val(person.PersonId).change();
            } else {
                AssignWorkstation.val(0).change();
            }
            locationId = w.w.LocationId;
        }

        WorkstationDialog.currentPoint = point;
        WorkstationDialog.saveCallback = function (result) { if (callback != undefined) return callback(result); };
        WorkstationDialog.locationId = locationId;

        this.dialog('open');
    };

    // PERSON DIALOG
    var PersonInputs = $('#PersonInputs');

    PersonDialog.dialog({
        title: "People",
        autoOpen: false,
        buttons: {
            Cancel: function () {
                PersonDialog.dialog('close');
            },
            Save: function () {
                var name = PersonInputs.find('.person-name').val();
                var extension = PersonInputs.find('.person-extension').val();

                var validationMessage = '';
                if (name.length == 0) {
                    validationMessage += 'Name is required. ';
                }
                if (extension.length == 0) {
                    validationMessage += 'Extension is required. ';
                }
                if (validationMessage.length > 0) {
                    alert(validationMessage);
                    return;
                }

                if (!CurrentContext.SavePerson(name, extension)) {
                    alert('Unable to save person!');
                    return;
                }

                PersonDialog.dialog('close');
            }
        },
        modal: true,
        position: { my: 'center top', at: 'center top+5' }
    });
    PersonDialog.open = function (personId) {
        if (!CurrentContext.IsAdmin()) {
            return;
        }

        if (personId === undefined) {
            PersonInputs.find('.person-name').val('');
            PersonInputs.find('.person-extension').val('');
        } else {
            var p = CurrentContext.GetPerson(personId);
            PersonInputs.find('.person-name').val(p.Name);
            PersonInputs.find('.person-extension').val(p.PhoneExtension);
        }

        this.dialog('open');
    }
    PersonDialog.close = function () {
        this.dialog('close');
    }
}