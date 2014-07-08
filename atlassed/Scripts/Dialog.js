Dialog = {
    BuildingDialog : null,
    SpaceDialog : null,
    WorkstationDialog : null,
    PersonDialog : null,
    init : function() {
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
            title: "Add/Edit Location",
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
                        vMessage += 'Name is required';
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

                    //Stubbing temporarily for demo
                    /*if (w == null) {
                        alert("Error saving workstation");
                        return;
                    }*/

                    AddingObject = false;
                    if (success) {
                        $(this).dialog('close');
                    }
                    //Changing for demo
                    //WorkstationDialog.saveCallback(w);
                    WorkstationDialog.saveCallback(1);
                    /*setTimeout(function () {
                        ShowDetails(TYPE_WORKSTATION, w.LocationId);
                    }, 50);*/
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
}

