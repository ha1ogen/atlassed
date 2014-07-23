Dialog = {
    BuildingDialog : null,
    SpaceDialog : null,
    EntityDialog : null,
    PersonDialog : null,
    init : function() {
        BuildingDialog = $('#BuildingDialog');
        SpaceDialog = $('#SpaceDialog');
        EntityDialog = $('#EntityDialog');
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

                    var name = "'" + $("#BuildingNameValue").val() + "'";
                    var code = "'" + $("#BuildingCodeValue").val()+ "'";
                    var type = "'" + $("#BuildingTypeValue").val()+ "'";
                    var faculty ="'" +  $("#BuildingFacultyValue").val()+ "'";
                    var address = "'" + $("#BuildingAddressValue").val()+ "'";

                    var returnData = CurrentContext.AddBuilding(name, code, type, faculty, address);

                    if (returnData !== null) {
                        var floors=[];
                        FloorList.find('tbody').children().each(function (i, f) {
                            var n = $(f).find('.floor-number').val()
                            floors.push(n);
                        });
                        var floorReturnData = CurrentContext.AddFloor(returnData.BuildingId, floors);

                        if (floorReturnData !== null) {
                            var  i = 0;
                            FloorList.find('tbody').children().each(function (i, f) {
                                var file = $(f).find('.floor-filename')[0].files[0];
                                if (file != null) {
                                    uploadFile(file, 'api/upload/images/map/' + floorReturnData[i].MapId);
                                    i++;    
                                }
                            });                            
                        }
                    }

                    Main.LoadBuildings(false);

                    BuildingDialog.dialog('close');
                }
            },
            modal: true,
            position: { my: 'center top', at: 'center top+5' }
        });
        BuildingDialog.open = function (buildingId) {
            if (!CurrentContext.IsAdmin()) {
                return;
            }

            removeAllFloors();

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

        // WORKSTATION
        $("#EntityList").change(function() {
            Dialog.changeEntityDialogClass(this.value);
        });

        EntityDialog.saveCallback = undefined;
        EntityDialog.dialog({
            title: "Add/Edit Location",
            autoOpen: false,
            buttons: {
                Cancel: function () {
                    $(this).dialog('close');
                    EntityDialog.saveCallback(null);
                },
                Save: function () {
                    var classIndex;
                    if (EntityDialog.addingObject) {
                        classIndex = $('#EntityList').val();
                    }
                    else {
                        var classes = CurrentContext.GetEntityClasses();
                        for (var id in classes) {
                            if (classes[id].ClassName === EntityDialog.ClassName) 
                                classIndex = parseInt(id);
                        }
                    }

                    var metafields = [];
                    var metaFieldInputs = $("#MetaFieldsWrapper input");

                    $.each(metaFieldInputs, function (i, v) {
                        metafields.push(v.value);
                    });

                    var w = CurrentContext.AddEditWorkstation(EntityDialog.addingObject, 
                        classIndex, metafields, EntityDialog.currentPoint, EntityDialog.entityId);
                    
                    if (w == null && !EntityDialog.addingObject) {
                        var o = getObjectByLocationId(EntityDialog.locationId);
                        var n = 0;
                        $.each(o.locationObj.MetaProperties, function (i, e) {
                            e.Value = metafields[n];
                            n++;
                        });
                    }

                    $(this).dialog('close');
                    EntityDialog.saveCallback(w);
                }
            },
            modal: true,
            position: { my: 'center top', at: 'center top+5' }
        });
        EntityDialog.open = function (parentLocationId, w, point, callback) {
            if (!CurrentContext.IsAdmin()) {
                return;
            }

            EntityDialog.saveCallback = function (result) { if (callback != undefined) return callback(result); };
            EntityDialog.locationId = parentLocationId;

            if (point)
                EntityDialog.currentPoint = point;
            else 
                EntityDialog.currentPoint = w.locationObj.EntityCoordinates[0];

            if (w !== null && w.locationObj !== null) {
                Dialog.changeEntityDialogClass(null, w.locationObj.ClassName);
                EntityDialog.addingObject = false;
                EntityDialog.ClassName = w.locationObj.ClassName;
                EntityDialog.entityId = w.locationObj.EntityId;
                $('#EntityListWrapper').hide();
                var metaProperties = w.locationObj.MetaProperties;
                for (var fieldName in w.locationObj.MetaProperties) {
                    var input = $("#" + fieldName + "Value");
                    input.val(metaProperties[fieldName].Value);
                }
                point = w.locationObj.EntityCoordinates[0];
            }
            else {
                EntityDialog.addingObject = true;
                $('#EntityListWrapper').show();
                Dialog.changeEntityDialogClass(null, 'Classroom');
            }
            this.dialog('open');
        };
    },
    changeEntityDialogClass: function (classIndex, newClass){
        var MetaFieldsWrapper = $("#MetaFieldsWrapper");
        MetaFieldsWrapper.empty();
        var classes = CurrentContext.GetEntityClasses();
        if (classIndex !== null) {
            for (var j = 0; j < classes[classIndex].MetaFields.length; j++) {
                var metaField = classes[classIndex].MetaFields[j];
                MetaFieldsWrapper.append('<br/><label id="' + metaField.FieldName + '">' + 
                    metaField.FieldLabel + ':</label>' + 
                    '<input id="' + metaField.FieldName + 'Value"></input>');
            } 
        }
        else {
            $.each(classes, function (i, v) {
                if (v.ClassName === newClass) {
                    for (var j = 0; j < v.MetaFields.length; j++) {
                        var metaField = v.MetaFields[j];
                        MetaFieldsWrapper.append('<br/><label id="' + metaField.FieldName + '">' + 
                            metaField.FieldLabel + ':</label>' + 
                            '<input id="' + metaField.FieldName + 'Value"></input>');
                    } 
                }
                
            });
        }
    }
}

