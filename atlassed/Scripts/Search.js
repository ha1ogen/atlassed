var SearchContainer,
    SearchInput,
    SearchResultContainer,
    SearchResultSummary,
    SearchResultsDetailsCard,
    SearchResultList,
    SearchResultTemplate,
    SearchResultDetail;

function initSearch() {
	SearchContainer = $('#Search');
    SearchInput = $('#SearchBox input');
    SearchResultContainer = $('#SearchResults');
    SearchResultSummary = $('#SearchResultSummary');
    SearchResultsDetailsCard = $('#DetailsCard');
    SearchResultList = $('#SearchResultList');
    SearchResultTemplate = $('#SearchResultTemplate');
    SearchResultDetail = $('#SearchResultDetail');

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