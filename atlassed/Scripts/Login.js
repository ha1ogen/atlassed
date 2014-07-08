Login = {
	init : function() {
		$('#login').click(function () {
	        Login.login();    
	    });
	    $('#logout').click(function () {
	        Login.logout();    
	    });
	},
	login : function () {
	    $('#LoginWrapper').css('visibility','hidden');
	    $('#AppWrapper').css('visibility','visible');
	    if ($('#username').val() == 'admin'){
	        admin = true;
	    }
	    else{
	        admin = false;
	        $('#Toolbar').css('visibility','hidden');
	    }    
	},
	logout : function () {
	    location.reload();  
	}
}
