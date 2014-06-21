var T_SELECT = 'T_SELECT',
    T_DRAW_RECT = 'T_DRAW_RECT',
    T_DRAW_POLY = 'T_DRAW_POLY',
    T_TAG_WORKSTATION = 'T_TAG_WORKSTATION',
    T_ERASE = 'T_ERASE';
var admin = false;
window.CurrentContext = new (function () {
    var self = this;
    $().ready(function () {
        //var admin = $('[id$=_ADMIN]').val() == 1;
        //$('[id$=_ADMIN]').remove();
        self.IsAdmin = function () { return admin; };

        self.GetAllBuildings();
    });

    // Building
    this.CurrentBuildingId = 0;
    this.EditBuildingId = 0;
    var _buildings = 
                [{  BuildingId:'1', 
                    BuildingName:'University', 
                    BuildingAddress: '200 University Avenue West Waterloo',
                    Floors:[{   FloorNumber:'1', 
                                FloorId:'11', 
                                Filename:'phase1_1stfloor.gif'},
                            {   FloorNumber:'2', 
                                FloorId:'12', 
                                Filename:'phase1_2ndfloor.gif'}]}, 
                            
                {   BuildingId:'2',
                    BuildingName:'Company', 
                    BuildingAddress: '1 Yonge Street Toronto',
                    Floors:[{   FloorNumber:'1', 
                                FloorId:'21', 
                                Filename:'Atlanta7thfloor.gif'},
                            {   FloorNumber:'2', 
                                FloorId:'22', 
                                Filename:'Atlanta8thfloor.gif'},
                            {   FloorNumber:'3', 
                                FloorId:'23', 
                                Filename:'phase5b_2ndfloor.gif'}]}];

    this.GetAllBuildings = function () {
        /*if (_buildings == null) {
            ajax({
                webservice: 'Main',
                func: 'GetBuildings',
                async: false,
                success: function (data) {
                    _buildings = data;
                }
            });
        }*/
        return _buildings;
    }

    this.GetBuilding = function (buildingId) {
        var buildings = this.GetAllBuildings();
        for (var i = 0; i < buildings.length; i++) {
            if (buildings[i].BuildingId == buildingId) {
                return buildings[i];
            }
        }

        return null;
    }

    this.AddBuilding = function (name, address) {
        var b = null;
        ajax({
            webservice: 'Admin',
            func: 'AddBuilding',
            async: false,
            params: {
                name: name,
                address: address
            },
            success: function (data) {
                b = data;
            }
        });
        return b;
    }

    this.SaveBuilding = function (name, address, floors) {
        if (!this.IsAdmin()) {
            return false;
        }

        var success = false;
        ajax({
            webservice: 'Admin',
            func: 'SaveBuilding',
            async: false,
            params: {
                buildingId: this.EditBuildingId,
                name: name,
                address: address,
                floors: floors
            },
            success: function (data) {
                if (data !== null) {
                    _buildings = data;
                    success = true;
                }
            }
        });

        // remove any removed floors from the dropdown
        //var newFloors = 
        //if (success && this.EditBuildingId == this.CurrentBuildingId) {
        //    SelectFloor.children().each(function (i, o) {

        //    });
        //}

        return success;
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
            SelectBuilding.find('option[value=' + buildingId + ']').remove();
        }

        return success;
    }

    // Floors
    this.CurrentFloorId = 0;

    function getFloor(floorId) {
        var floor = null;
        var buildings = self.GetAllBuildings();
        $.each(buildings, function (i, b) {
            $.each(b.Floors, function (j, f) {
                if (f.FloorId == floorId) {
                    floor = f;
                }
            });
        });
        return floor;
    }

    this.LoadFloor = function (floorId, callback) {
        var internalCallback = function (success) { if (callback != undefined) callback(success); };
        var f = getFloor(floorId);
        //self.CurrentBuildingId = f.BuildingId;
        if (SelectBuilding.val() != f.BuildingId) {
            SelectBuilding.val(f.BuildingId).change();
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

        data = [{locationId:'1', X: '100', Y:'100'}];
        //Stubbing
        LoadCanvas(f.Filename, data, internalCallback);
    };

    this.GetFloors = function (buildingId) {
        var building = this.GetBuilding(buildingId);
        if (building === null) {
            return null;
        }
        return building.Floors;
    }

    // Workstation
    this.CurrentWorkstation = 0;

    this.AddWorkstation = function (point, workspaceId, number, port, personId, callback) {
        var w = null;
        ajax({
            webservice: 'Admin',
            func: 'AddWorkstation',
            async: false,
            params: {
                point: point,
                workspaceId: workspaceId,
                number: number,
                port: port,
                personId: personId
            },
            success: function (data) {
                if (data !== null) {
                    w = data;
                }
            }
        });
        return w;
    }

    this.SaveWorkstation = function (workstationId, number, port, personId) {
        if (!this.IsAdmin()) {
            return false;
        }

        var w = null;
        ajax({
            webservice: 'Admin',
            func: 'SaveWorkstation',
            async: false,
            params: {
                number: number,
                port: port,
                personId: personId,
                workstationId: workstationId
            },
            success: function (data) {
                w = data;
            }
        });

        this.LoadFloor(this.CurrentFloorId);

        return w;
    }

    this.RemoveWorkstation = function (workstationId) {
        if (!this.IsAdmin() || isNaN(workstationId)) {
            return false;
        }

        var success = false;
        ajax({
            webservice: 'Admin',
            func: 'DeleteWorkstation',
            async: false,
            params: { workstationId: workstationId },
            success: function (data) {
                success = data;
            }
        });

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
        var results = [];
        for (var i = 0 ; i < _buildings.length; i++) {
            if (_buildings[i].BuildingName.search(query) >= 0) {
                results.push({PrimaryText:_buildings[i].BuildingName, 
                    SecondaryText:"Building"})
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