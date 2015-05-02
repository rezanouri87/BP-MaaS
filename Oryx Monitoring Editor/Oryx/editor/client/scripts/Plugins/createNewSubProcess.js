;
var subprocessURL;

function createSubprocessURL(child, side)
	{
		var shape = shapesMap.get(child.parentNode.id); 
		var raiseEvent = shape._delegateEvent.bind(shape); 		
		raiseEvent({type:'create.new.subprocess',shape:shape });				
		setTimeout(function(){			
			subprocessURL = generateURL(subprocessURL)
			if(side == 'L'){
				shape.setProperty('oryx-subprocesslinkl', subprocessURL);
			}else if(side == 'R'){
				shape.setProperty('oryx-subprocesslinkr', subprocessURL);
			}else if (side == 'C'){
				shape.setProperty('oryx-subprocesslinkc', subprocessURL);
			}			
			shape.update();		
			shape.refresh();						
		},3000);		
	}
		
function generateURL(oldFormat){
	var n = oldFormat.lastIndexOf("/");
    oldFormat = oldFormat.substr(0, n);
    n = oldFormat.lastIndexOf("/");
    oldFormat = oldFormat.substr(n+1);
	//return 'http://ec2-54-186-28-251.us-west-2.compute.amazonaws.com:8080/oryx/editor;monitoring#/model/' + oldFormat + '/';	
	var parser = document.createElement('a');
	parser.href = document.URL;	
	return 'http://'+ parser.host + '/oryx/editor;monitoring#/model/' + oldFormat + '/';	
}		
		
if (!ORYX.Plugins) 
    ORYX.Plugins = new Object();

ORYX.Plugins.createNewSubProcess = ORYX.Plugins.AbstractPlugin.extend({
	
    facade: undefined,
	
	processURI: undefined,
	
    construct: function(facade){
		this.facade = facade;
		
		// Register on event for executing commands --> store all commands in a stack		 
		// --> Execute
		this.facade.registerOnEvent('create.new.subprocess',this.save.bind(this,false));			
	},	
	
    saveSynchronously: function(){
            
		// Reset changes
		this.changeDifference = 0;
		var reqURI = "/backend/poem/new";
		
		
		// Get the serialized svg image source
        var svgDOM = '<svg xmlns="http://www.w3.org/2000/svg" xmlns:oryx="http://oryx-editor.org" id="oryx_B6776EF1-ECF5-4AF0-AD6D-CF2E97A32E34" width="50" height="50" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:svg="http://www.w3.org/2000/svg"><defs/><g stroke="black" font-family="Verdana, sans-serif" font-size-adjust="none" font-style="normal" font-variant="normal" font-weight="normal" line-heigth="normal" font-size="12"><g class="stencils" transform="translate(25, 25)"><g class="me"/><g class="children"/><g class="edge"/></g></g></svg>';
		
		this.serializedDOM = '{"resourceId":"oryx-canvas123","properties":{},"stencil":{"id":"Diagram"},"childShapes":[],"bounds":{"lowerRight":{"x":1485,"y":1050},"upperLeft":{"x":0,"y":0}},"stencilset":{"url":"/oryx/stencilsets/monitoringRules/monitoringRules.json","namespace":"http://b3mn.org/stencilset/monitoringRules#"},"ssextensions":[]}'
		
		
		// Get the stencilset
		var ss = this.facade.getStencilSets().values()[0]
	
		// Define Default values
		var defaultData = {title:ORYX.I18N.Save.newProcess, summary:'', type:ss.title(), url: reqURI, namespace: ss.namespace() }

		
		//added changing title of page after first save
		window.document.title = defaultData.title + " - Oryx";
		
		// Send the request out
		this.sendSaveRequest( reqURI, { data: this.serializedDOM, svg: svgDOM, title: defaultData.title, summary: '', type: defaultData.namespace }, false);
		subprocessURL = this.processURI;		
    },
	
	sendSaveRequest: function(url, params, forceNew){

		// Send the request to the server.
		new Ajax.Request(url, {
                method: 'POST',
                asynchronous: false,
                parameters: params,
			onSuccess: (function(transport) {
				var loc = transport.getResponseHeader("location");
				if (loc) {
					this.processURI = loc;
				}
				else {
					this.processURI = url;
				}
				
				//raise saved event
				this.facade.raiseEvent({
					type:ORYX.CONFIG.EVENT_MODEL_SAVED
				});
				//show saved status
				this.facade.raiseEvent({
						type:ORYX.CONFIG.EVENT_LOADING_STATUS,
						text:ORYX.I18N.Save.saved
					});
			}).bind(this),
			onFailure: (function(transport) {
				// raise loading disable event.
                this.facade.raiseEvent({
                    type: ORYX.CONFIG.EVENT_LOADING_DISABLE
                });


				Ext.Msg.alert(ORYX.I18N.Oryx.title, ORYX.I18N.Save.failed);
				
				ORYX.Log.warn("Saving failed: " + transport.responseText);
			}).bind(this),
			on403: (function(transport) {
				// raise loading disable event.
                this.facade.raiseEvent({
                    type: ORYX.CONFIG.EVENT_LOADING_DISABLE
                });


				Ext.Msg.alert(ORYX.I18N.Oryx.title, ORYX.I18N.Save.noRights);
				
				ORYX.Log.warn("Saving failed: " + transport.responseText);
			}).bind(this)
		});
				
	},
    
    /**
     * Saves the current process to the server.
     */
    save: function(forceNew, event){
    
        
        // asynchronously ...
        window.setTimeout((function(){
        
            // ... save synchronously
           this.saveSynchronously();
            
        }).bind(this), 10);

        return true;
    }	
});