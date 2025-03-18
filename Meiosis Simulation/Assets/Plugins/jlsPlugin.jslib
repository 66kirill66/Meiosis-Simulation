mergeInto(LibraryManager.library,
{
  
  callTNITfunction: function() 
  {
    globals.init();
  },

  onResetDone: function() 
  {
    globals.onResetDone();
  },

  EntityClick: function(id) 
  {
    globals.entityClick(id);
  },

  MeiosisBegins: function(id) 
  {
    globals.MeiosisBegins(id);
  },

  SendLangCode: function()
	{
		globals.SendLangCode();
	},

  SendEvent: function(name)
	{
		var text = UTF8ToString(name)
		globals.SendEvent(text);
	},

  OnWereFormed: function(id) 
  {
    globals.OnWereFormed(id);
  },

  FirstDivisionDone: function(id) 
  {
    globals.FirstDivisionDone(id);
  },

   ReplacementIsOver: function(id) 
  {
    globals.ReplacementIsOver(id);
  },

  

  


  


  UpdatePositionedLocation: function(id, location)
	{
		var text = UTF8ToString(location)
		globals.UpdatePositionedLocation(id, text);
	},





  
  

  




  

  

 



  
});
