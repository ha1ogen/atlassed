var SearchContainer,
    SearchInput,
    SearchResultContainer,
    SearchResultSummary,
    SearchResultsDetailsCard,
    SearchResultList;

function initSearch() {
	SearchContainer = $('#Search');
    SearchInput = $('#SearchBox input');
    SearchResultContainer = $('#SearchResults');
    SearchResultSummary = $('#SearchResultSummary');
    SearchResultsDetailsCard = $('#DetailsCard');
    SearchResultList = $('#SearchResultList');

   SearchContainer.closeAll = function () {
   		HidePin();
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
        HidePin();
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
        if (SearchResultList.isEmpty()) {
        	$('#CloseCard').text("Close");
        }
        else {
        	$('#CloseCard').text("Back");
        }

        $('#DetailsCardTitle').text(title);
        // fill details table
        var table = $('#DetailsCardTable');
        table.empty();
        $.each(details || [], function (label, value) {
        	if (value === "NULL") value = "n/a";
            table.append('<tr><td>' + label + ':</td><td>' + value + '</td></tr>');
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

        SearchResultContainer.close('up');
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

    SearchInput
        .bind('input', function (evt) {
            var query = $(this).val();
            SearchResultContainer.switchPane('list');

            if (query.length === 0) {
                CurrentContext.ClearSearch();

                SearchContainer.closeAll();
                return;
            }
 
            ajax({
                webservice: 'api/mapEntities',
                params: { q: query },
                success: FormatSearchResults,
                failure : function () {
                	var results = CurrentContext.search(query);
            		FormatSearchResults(results);	
                },
                error : function () {
                	var results = CurrentContext.search(query);
            		FormatSearchResults(results);	
                }
            });
        })
        .focus(function () {
            OnFocus_SearchArea();
        });

    $('#ClearSearch').click(function () {
        SearchResultSummary.setMatches(0);
        SearchResultList.empty();
        SearchContainer.closeAll();
        CurrentContext.ClearSearch();
        SearchInput.val('');
    });

    SearchResultsDetailsCard.find('#CloseCard').click(function () {
        if (SearchResultList.isEmpty()) {
        	SearchResultsDetailsCard.clear();
        }
        else {
        	SearchResultContainer.switchPane('list');
        }
    });
}

function FormatSearchResults (results) {

    CurrentContext.setSearchResults(results);

    SearchResultList.empty();

    if (results.length > 0) {
        $.each(results, function (i, e) {
            AppendSearchResult(i, e);
        });
    }

    SearchResultSummary.setMatches(results.length);
    SearchResultSummary.open();
    SearchResultContainer.switchPane('list', SearchResultContainer.open);

    $('.search-result').click(function (e) {
        var self = this;
        clickResult.call(this, e, function (type, id) {
            Main.ShowDetails(type, id);
        });
        return false;
    });

    function clickResult(e, callback) {
        var innerCallback = function (success, type, id) {
            if (callback != undefined) {
                callback(type, id);
            }
        };
        var result = $(this).closest('.search-result'),
            id = result.data('result-id'),
            type = result.data('result-type'),
            secondaryId = result.data('result-secondaryid');

        $.each(CurrentContext.GetEntityClasses(), function(i, v) {
        	if (v.ClassName.toLowerCase() == type.toLowerCase()) {
        		type = 'Entity';
        		return false;
        	}
        });

        switch (type) {
            case 'Building':
                Main.GoToBuilding(id);
                innerCallback(true, type, id);
                break;
            case 'Person':
                if (secondaryId == 0) return;
            case 'Space':
            case 'Entity':
            case 'Workstation':
                Main.GoToFloor(secondaryId, function (success) {
                    if (success) {
                        selectObjectByLocationId(id);
                    }
                    innerCallback(success, type, id);
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
}

function AppendSearchResult(i, resultData) {
    var result = $('#SearchResultTemplate').clone();
    // reset template
    result.css('display', '');
    // properties
    result.attr('data-result-id', resultData.PrimaryId);
    result.attr('data-result-type', resultData.ClassName);
    result.attr('data-result-secondaryid', resultData.SecondaryId);
    if (resultData.Point !== undefined) {
        result.attr('data-result-point', resultData.Point.X + ',' + resultData.Point.Y);
    }
    // set visible elements
    result.find('.primary-text').text(resultData.PrimaryText);
    result.find('.secondary-text').text(resultData.ClassName);

    SearchResultList.append(result);
}

function OnFocus_SearchArea() {
    if (CurrentContext.setSearchResults().length > 0) {
        SearchContainer.openAll();
    }
}

function OnBlur_SearchArea() {
    SearchContainer.closeAll();
}

function CenterSearch() {
	SearchContainer
	    .css({
	        left: centerX(SearchContainer),
	        top: centerY(SearchContainer) - 100
	    });
}