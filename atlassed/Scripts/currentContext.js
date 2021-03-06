﻿var T_SELECT = 'T_SELECT',
    T_DRAW_RECT = 'T_DRAW_RECT',
    T_DRAW_POLY = 'T_DRAW_POLY',
    T_TAG_WORKSTATION = 'T_TAG_WORKSTATION',
    T_ERASE = 'T_ERASE',
    T_EDIT = 'T_EDIT';
var admin = false;
window.CurrentContext = new (function () {
    var self = this;
    $().ready(function () {
        self.IsAdmin = function () { return admin; };
    });

    // Building
    this.CurrentBuildingId = 0;
    this.EditBuildingId = 0;
    var entityClasses = {};
    var _buildings = [];

    this.LoadEntityClasses = function () {
        ajax({
            webservice: 'api/mapEntityClasses',
            async: true,
            success: function (data){
                Main.LoadEntityDialog(data);
            }
        });    
    }

    this.GetEntityClasses = function () {
        return entityClasses;
    }

    this.GetBuildingsFromServer = function () {
        ajax({
            webservice: 'api/buildings',
            async: false,
            success: function (data){
                _buildings = data;
            }
        });
        return _buildings;
    }

    this.GetBuildings = function (buildingId) {
        return _buildings;
    }

    this.GetBuilding = function (buildingId) {
        for (var i = 0; i < _buildings.length; i++) {
            if (_buildings[i].BuildingId == buildingId) {
                return _buildings[i];
            }
        }
        return null;
    }


    this.AddBuilding = function (name, code, type, faculty, address) {
        if (!this.IsAdmin()) {
            return false;
        }

        var w = null;
        ajax({
            webservice: 'api/buildings',
            type:'post',
            async: false,
            params: {
                buildingAddress : address,
                campusmapid : 2,
                entityCoordinates : [{x:0,y:0}],
                metaproperties: {
                    buildingname: name, 
                    buildingcode: code, 
                    buildingtype: type, 
                    buildingfaculty: faculty
                }
            },
            success: function (data) {
                if (data !== null) {
                    _buildings.push(data);
                    w = data;
                }
            }
        });

        return w;
    }

    this.RemoveBuilding = function (buildingId) {
        if (!this.IsAdmin()) {
            return false;
        }

        var success = false;
        ajax({
            webservice: 'Admin',
            func: 'DeleteBuilding',
            async: false,
            params: { buildingId: buildingId },
            success: function (data) {
                if (data) {
                    $.each(_buildings, function (i, b) {
                        if (b.BuildingId = buildingId) {
                            _buildings[i] = undefined;
                            success = true;
                        }
                    });
                }
            }
        });

        if (success) {
            Tile.SelectBuilding.find('option[value=' + buildingId + ']').remove();
        }

        return success;
    }

    // Floors
    this.CurrentFloorId = 0;

    function getFloor(floorId) {
        var floor = null;
        ajax({
            webservice: 'api/floors/' + floorId,
            async: false,
            success: function (data){
                floor = data;
            }
        });
        return floor;
    }

    this.LoadFloor = function (floorId, callback) {
        var internalCallback = function (success) { if (callback != undefined) callback(success); };
        var f = getFloor(floorId);
        //self.CurrentBuildingId = f.BuildingId;
        if (Tile.SelectBuilding.val() != f.BuildingId) {
            Tile.SelectBuilding.val(f.BuildingId).change();
        }
        /*ajax({
            webservice: 'Main',
            func: 'LoadFloorData',
            async: false,
            params: {
                floorId: floorId
            },
            success: function (data) {
                LoadCanvas(f.Filename, data, internalCallback);
                self.CurrentFloorId = floorId;
            },
            failure: function (data) {
                internalCallback(false);
            },
            error: function (data) {
                internalCallback(false);
            }
        });*/

        ajax({
            webservice: 'api/maps/' + floorId + '/entities',
            async: false,
            success: function (data){
                LoadCanvas(f.MapFilename, data, internalCallback);
                self.CurrentFloorId = floorId;
            },
            failure: function (data) {
                internalCallback(false);
            },
            error: function (data) {
                internalCallback(false);
            }
        });
        //return _floors;

        /*if (floorId == '13'){
            data = [{locationObj: {id: '1', Number:'1', Name:'Room 307', Type:'Classroom', Description: 'Max Occupancy 150' ,AssignedPerson: null}, X: '285', Y:'425'},
            {locationObj: {id: '2', Number:'1', Name:'Room 301', Type:'Classroom', Description: 'Max Occupancy 175' ,AssignedPerson: null}, X: '886', Y:'392'},
            {locationObj: {id: '3', Number:'1', Name:'Room 305', Type:'Classroom', Description: 'Max Occupancy 150' ,AssignedPerson: null}, X: '508', Y:'643'}];
        }else{
            data = [{locationObj: {id: '1', Number:'1', Name:'Room', Description: 'Room' ,AssignedPerson: null}, X: '500', Y:'130'}];
        }*/
        //Stubbing
        //LoadCanvas(f.Filename, data, internalCallback);
    };

    this.GetFloors = function (buildingId) {
        /*var building = this.GetBuilding(buildingId);
        if (building === null) {
            return null;
        }
        return building.Floors;*/

        ajax({
            webservice: 'api/buildings/' + buildingId + '/floors',
            async: false,
            success: function (data){
                _floors = data;
            }
        });
        return _floors;
    }

    this.AddFloor = function (buildingId, floors) {
        if (!this.IsAdmin()) {
            return false;
        }
        var w = null;

        var params = [];

        $.each(floors, function (i, n) {
            params.push({
                floorordinal: parseInt(n),
                floorcode: n,
                floorlabel: n,        
                metaproperties : '{}'
            });
        });

        ajax({
            webservice: 'api/buildings/' + buildingId+ '/floors',
            async: false,
            type: 'post',
            params: {
                '' : params
            },
            success: function (data) {
                w = data;
            }
        });

        return w;
    }

    // Workstation
    this.CurrentWorkstation = 0;

    this.AddEditWorkstation = function (add, classIndex, metafieldsValues, coordinates, entityId) {
        if (!this.IsAdmin()) {
            return false;
        }
        var w = null;

        var metafieldsParams = {};
        var metafields = entityClasses[classIndex].MetaFields;
        for (var i = 0; i < metafields.length; i++) {
            if (metafields[i].FieldType === "INT") {
                metafieldsValues[i] = parseInt(metafieldsValues[i]);
            }
            else {
                metafieldsValues[i] = "'"+metafieldsValues[i]+"'";
            }
            metafieldsParams[metafields[i].FieldName] = metafieldsValues[i];
        }

       var params = {
            classname: entityClasses[classIndex].ClassName,
            mapId:  parseInt(this.CurrentFloorId),
            entityCoordinates: [{
                x : coordinates.X,
                y : coordinates.Y
            }],
            metaproperties : metafieldsParams
        };

        var type;
        if (add) {
            type = 'post';
        } 
        else {
            params.entityId = entityId;
            type = 'put';
        }

        ajax({
            webservice: 'api/mapEntities',
            async: false,
            type: type,
            params: params,
            success: function (data) {
                w = data;
            }
        });

        return w;
    }


    this.RemoveWorkstation = function (workstationId) {
        if (!this.IsAdmin() || isNaN(workstationId)) {
            return false;
        }

        var success = false;


        ajax({
            webservice: 'api/mapEntities/' + workstationId,
            async: false,
            type: 'delete',
            success: function (data){
                success = data;
            }
        });

        /*ajax({
            webservice: 'Admin',
            func: 'DeleteWorkstation',
            async: false,
            params: { workstationId: workstationId },
            success: function (data) {
                success = data;
            }
        });*/

        return success;
    }

    // Space
    this.AddSpace = function (topLeft, bottomRight, spaceName, spaceNumber) {
        var s = null;
        ajax({
            webservice: 'Admin',
            func: 'AddSpace',
            async: false,
            params: {
                topLeft: topLeft,
                bottomRight: bottomRight,
                name: spaceName,
                number: spaceNumber,
                floorId: this.CurrentFloorId
            },
            success: function (data) {
                s = data;
            }
        });
        return s;
    }

    this.SaveSpace = function (spaceId, spaceName, spaceNumber) {
        if (!this.IsAdmin()) {
            return false;
        }

        var s = null;
        ajax({
            webservice: 'Admin',
            func: 'SaveSpace',
            async: false,
            params: {
                spaceId: spaceId,
                name: spaceName,
                number: spaceNumber
            },
            success: function (data) {
                s = data;
            }
        });

        return s;
    }

    this.RemoveSpace = function (spaceId) {
        if (!this.IsAdmin() || isNaN(spaceId)) {
            return false;
        }

        var success = false;
        ajax({
            webservice: 'Admin',
            func: 'DeleteSpace',
            async: false,
            params: { spaceId: spaceId },
            success: function (data) {
                success = data;
            }
        });

        return success;
    }

    // Person
    this.CurrentPersonId = 0;
    var _people = null;
    var _peopleCache = [];
    function GetAllPeople() {
        var people;
        ajax({
            webservice: 'Admin',
            func: 'GetAllPeople',
            async: false,
            success: function (data) {
                people = data;
            }
        });
        return people;
    }

    this.GetPerson = function (personId) {
        var person = null;
        if (this.IsAdmin()) {
            if (_people === null) {
                _people = GetAllPeople();
            }
            // check if the personid has been cached
            if (_peopleCache[personId] !== undefined) {
                return _people[_peopleCache[personId]];
            }
            $.each(_people, function (i, p) {
                if (p.PersonId == personId) {
                    // cache this personId
                    _peopleCache[personId] = i;
                    person = p;
                }
            });
        } else {
            ajax({
                webservice: 'Main',
                func: 'GetPerson',
                async: false,
                params: { personId: personId },
                success: function (data) {
                    person = data;
                }
            });
        }
        return person;
    }

    this.GetUnassignedPeople = function (callback) {
        var async = true;
        var people = true;
        /*if (callback === undefined) {
            async = false;
            people = false;
            callback = function (data) {
                people = data;
            };
        }
        ajax({
            webservice: 'Admin',
            func: 'GetUnassignedPeople',
            async: async,
            success: callback
        });*/
        people = [{PersonId: '1', Name: 'TestPerson1'}, {PersonId:'2', Name: 'TestPerson2'}]
        return people;
    }

    this.SavePerson = function (name, extension) {
        if (!this.IsAdmin()) {
            return false;
        }

        var success = false;
        ajax({
            webservice: 'Admin',
            func: 'SavePerson',
            async: false,
            params: {
                name: name,
                extension: extension,
                personId: this.CurrentPersonId
            },
            success: function (data) {
                success = data;
            }
        });
        return success;
    }

    this.RemovePerson = function (personId) {
        if (!this.IsAdmin()) {
            return false;
        }


    }

    // TOOLBAR
    var _curentTool = T_SELECT;
    this.CurrentTool = function (currentTool) {
        if (currentTool === undefined) {
            return _curentTool;
        } else {
            _curentTool = currentTool;
            switch (currentTool) {
                case T_SELECT:
                    Canvas.defaultCursor = 'default';
                    break;
                case T_DRAW_RECT:
                    Canvas.defaultCursor = 'crosshair';
                    removeAllControls();
                    break;
                case T_TAG_WORKSTATION:
                    Canvas.defaultCursor = 'pointer';
                    removeAllControls();
                    break;
            }
        }
    };

    // SEARCH
    var _searchResults = [];
    this.setSearchResults = function (results) {
        if (results === undefined) {
            return _searchResults;
        } else {
            _searchResults = results;
        }
    }
    this.ClearSearch = function () {
        _searchResults = [];
    }

    this.search = function (query) {
        query.toLowerCase();
        var results = [];
        for (var i = 0 ; i < _buildings.length; i++) {
            var buildingName = _buildings[i].BuildingName.toLowerCase();
            if (buildingName.search(query) >= 0) {
                results.push({
                    Type:'Building',
                    PrimaryText : _buildings[i].BuildingName, 
                    SecondaryText : _buildings[i].BuildingAddress,
                    PrimaryId : _buildings[i].BuildingId})
            }
        }
        return results;
    }

    // SELECTION
    var _currentSelection = 0;
    this.CurrentSelection = function (currentSelection) {
        if (currentSelection === undefined) {
            return _currentSelection;
        } else {
            _currentSelection = currentSelection;
        }
    };
})();