var initialX = 2000;
var initialY = 100;
var vSeparator = 20;
var hSeparator = 500;
function getNodeWidth(name){
	name = name.toLowerCase();
	if(name === 'and' || name === 'or' || name === 'xor' || name === 'implies' || name === 'not' || name === 'segregationofduty'|| name === 'bindofduty' || name === 'performedbyrole' || name === 'performedbyresource' || name === 'waiting' ){
		return 240;
	}	
	else if(name === 'absence' || name === 'resume' || name === 'exactlytimes' || name === 'atleasttimes' || name === 'atmosttimes' || name === 'exists' || name === 'responsible'){
		return 200;
	}else{
		return 400;
	}
}

function getNodeHeight(name){
	name = name.toLowerCase();
	if(name === 'and' || name === 'or' || name === 'xor' || name === 'implies' || name === 'not' || name === 'segregationofduty'|| name === 'bindofduty' || name === 'performedbyrole' || name === 'performedbyresource' || name === 'waiting' ){
		return 120;
	}	
	else if(name === 'absence' || name === 'resume' || name === 'exactlytimes' || name === 'atleasttimes' || name === 'atmosttimes' || name === 'exists' || name === 'responsible'){
		return 120;
	}else{
		return 75;
	}	
}


ORYX.Plugins.RulesSummary = Clazz.extend({

    // Defines the facade
    facade: undefined,
	json:undefined,
    // Constructor 
    construct: function (facade) {

        this.facade = facade;

        // Offers the functionality of undo                
        this.facade.offer({
            name: 'RuleOverview',
            description: 'Generate Rule Overview',
            icon: ORYX.PATH + "images/summary.png",
            group: ORYX.I18N.Save.group,
            functionality: this.showModel.bind(this),
            isEnabled: function () {
                return true;
            }.bind(this),
            index: 2
        });

    },
	
	openLinkInNewTab: function(url){
		var a = document.createElement('a');
		a.setAttribute('href', url);
		a.setAttribute('target', '_blank');
		document.body.appendChild(a);
		a.click();	
	},
	
	saveSynchronously: function(){
            
		// Reset changes
		this.changeDifference = 0;
		var reqURI = "/backend/poem/new";
		
		
		// Get the serialized svg image source
        var svgDOM = '<svg xmlns="http://www.w3.org/2000/svg" xmlns:oryx="http://oryx-editor.org" id="oryx_B6776EF1-ECF5-4AF0-AD6D-CF2E97A32E34" width="50" height="50" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:svg="http://www.w3.org/2000/svg"><defs/><g stroke="black" font-family="Verdana, sans-serif" font-size-adjust="none" font-style="normal" font-variant="normal" font-weight="normal" line-heigth="normal" font-size="12"><g class="stencils" transform="translate(25, 25)"><g class="me"/><g class="children"/><g class="edge"/></g></g></svg>';
				
		this.serializedDOM = Object.toJSON(json);
		
		// Get the stencilset
		var ss = this.facade.getStencilSets().values()[0]
	
		// Define Default values
		var defaultData = {title:ORYX.I18N.Save.newProcess, summary:'', type:ss.title(), url: reqURI, namespace: ss.namespace() };

		
		//added changing title of page after first save
		window.document.title = defaultData.title + " - Oryx";
		
		// Send the request out
		this.sendSaveRequest( reqURI, { data: this.serializedDOM, svg: svgDOM, title: defaultData.title, summary: '', type: defaultData.namespace }, false);		
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
				// open new tab	
				console.log(this.processURI);	
				this.openLinkInNewTab(this.processURI);	
				
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
       	
	formulateAjaxCallURL: function (oldURL) {
        // oldURL formate like http://hostname:port/oryx/editor;monitoring#/model/178/
        // and need to be converted like http://hostname:port/backend/poem/model/176/json
		if(oldURL.indexOf("/", oldURL.length - 1) === -1){
			oldURL += "/";
		}
        var n = oldURL.lastIndexOf("/");
        oldURL = oldURL.substr(0, n);
        n = oldURL.lastIndexOf("/");
        var modelNumber = oldURL.substr(n + 1);

        var parser = document.createElement('a');
        parser.href = oldURL;
        return 'http://' + parser.host + '/backend/poem/model/' + modelNumber + '/json';
    },
	
	getJSONLink: function(linkId, targetId, lowerRight, upperLeft, pDocker, cDocker){
		return { "resourceId": linkId, "properties":{ }, "stencil":{ "id":"Precedes" }, "childShapes":[ ], "outgoing":[ { "resourceId": targetId } ], "bounds":{ "lowerRight":{ "x": lowerRight.x, "y": lowerRight.y }, "upperLeft":{ "x": upperLeft.x, "y": upperLeft.y } }, "dockers":[ { "x":pDocker.x, "y": pDocker.y }, { "x":cDocker.x, "y":cDocker.y } ], "target":{ "resourceId": targetId }};	 	 
	},
	
	addToJSON: function(jsonData, parentIndex, direction){
		var node;
		if(parentIndex	 == undefined){
			// create JSON file
			json = { 'resourceId':'oryx-canvas123', 'properties':{ }, 'stencil':{ 'id':'Diagram' }, "childShapes":[ ], 'bounds':{ 'lowerRight':{ 'x':4000, 'y':2000 }, 'upperLeft':{ 'x':0, 'y':0 } }, 'stencilset':{ 'url':'/oryx/stencilsets/monitoringRules/monitoringRules.json', 'namespace':'http://b3mn.org/stencilset/monitoringRules#' }, 'ssextensions':[ ] };			
			json.childShapes[0] = {};
			json.childShapes[0].resourceId = jsonData.childShapes[0].resourceId;
			json.childShapes[0].properties = jsonData.childShapes[0].properties;
			json.childShapes[0].stencil = {};
			json.childShapes[0].stencil.id = jsonData.childShapes[0].stencil.id;
			json.childShapes[0].childShapes = [];
			json.childShapes[0].outgoing = [];
			json.childShapes[0].bounds = {};
			json.childShapes[0].bounds.upperLeft = {};
			json.childShapes[0].bounds.lowerRight = {};
			json.childShapes[0].bounds.upperLeft.x = initialX;
			json.childShapes[0].bounds.upperLeft.y = initialY;
			json.childShapes[0].bounds.lowerRight.x = initialX + getNodeWidth(json.childShapes[0].stencil.id);
			json.childShapes[0].bounds.lowerRight.y = initialY + getNodeHeight(json.childShapes[0].stencil.id);
			return 0;	
		}else{
			// append to existing XML file
			var childIndex = json.childShapes.length;
			json.childShapes[childIndex] = {};
			var child = json.childShapes[childIndex];			
			var parent = json.childShapes[parentIndex];						
			var psid = parent.resourceId;			
			
			child.resourceId = jsonData.childShapes[0].resourceId;
			child.properties = jsonData.childShapes[0].properties;
			child.stencil = {};
			child.stencil.id = jsonData.childShapes[0].stencil.id;
			child.childShapes = [];
			child.outgoing = [];			
			var pOutLength = parent.outgoing.length;
			parent.outgoing[pOutLength] = {};
			child.bounds = {};
			child.bounds.upperLeft = {};
			child.bounds.lowerRight = {};
			
			if(direction === 'L'){
				// set parent link 								
				parent.outgoing[pOutLength].resourceId = psid.substring(0, psid.length -1) + 'L';
				// set shape boundries
				child.bounds.upperLeft.x = parent.bounds.upperLeft.x - getNodeWidth(child.stencil.id);
				child.bounds.upperLeft.y = parent.bounds.lowerRight.y + vSeparator;
				child.bounds.lowerRight.x = parent.bounds.upperLeft.x;
				child.bounds.lowerRight.y = parent.bounds.lowerRight.y + vSeparator + getNodeHeight(child.stencil.id);	
			}else if (direction === 'R'){
				// set parent link 								
				parent.outgoing[pOutLength].resourceId = psid.substring(0, psid.length -1) + 'R';
				// set shape boundries
				child.bounds.upperLeft.x = parent.bounds.lowerRight.x ;
				child.bounds.upperLeft.y = parent.bounds.lowerRight.y + vSeparator;
				child.bounds.lowerRight.x = parent.bounds.lowerRight.x + getNodeWidth(child.stencil.id);
				child.bounds.lowerRight.y = parent.bounds.lowerRight.y + vSeparator + getNodeHeight(child.stencil.id);					
			}else{
				// set parent link				 			
				parent.outgoing[pOutLength].resourceId = psid.substring(0, psid.length -1) + 'C';
				// set shape boundries
				child.bounds.upperLeft.x = parent.bounds.upperLeft.x ;
				child.bounds.upperLeft.y = parent.bounds.lowerRight.y + vSeparator;
				child.bounds.lowerRight.x = parent.bounds.upperLeft.x + getNodeWidth(child.stencil.id);
				child.bounds.lowerRight.y = parent.bounds.lowerRight.y + vSeparator + getNodeHeight(child.stencil.id);
			}
						
			// set link shape data
			var linkjson;
			var lowerRight = {};
			var upperLeft = {};
			var pDocker = {};
			var cDocker = {};
			if(direction === 'L'){					
				if(parentIndex === 0){
					// special case for first level only
					child.bounds.upperLeft.x -=  hSeparator;
					child.bounds.lowerRight.x -= hSeparator;
				}
				upperLeft.x = child.bounds.upperLeft.x + (getNodeWidth(child.stencil.id) / 2);
				upperLeft.y = parent.bounds.upperLeft.y + (getNodeHeight(parent.stencil.id) / 2);
				lowerRight.x = parent.bounds.upperLeft.x;
				lowerRight.y = child.bounds.upperLeft.y;
				pDocker.x = 0;
				pDocker.y = getNodeHeight(parent.stencil.id) / 2;
				cDocker.x = getNodeWidth(child.stencil.id) / 2;
				cDocker.y = 0;
				linkjson = this.getJSONLink(psid.substring(0, psid.length -1) + 'L', child.resourceId, lowerRight, upperLeft, pDocker, cDocker);	
			}else if(direction === 'R'){				
				if(parentIndex === 0){
					// special case for first level only
					child.bounds.upperLeft.x +=  hSeparator;
					child.bounds.lowerRight.x += hSeparator;				
				}
				upperLeft.x = parent.bounds.lowerRight.x
				upperLeft.y = parent.bounds.upperLeft.y + (getNodeHeight(parent.stencil.id) / 2);
				lowerRight.x = child.bounds.upperLeft.x + (getNodeWidth(child.stencil.id) / 2);
				lowerRight.y = child.bounds.upperLeft.y;
				pDocker.x = getNodeWidth(parent.stencil.id);
				pDocker.y = getNodeHeight(parent.stencil.id) / 2;
				cDocker.x = getNodeWidth(child.stencil.id) / 2;
				cDocker.y = 0;
				linkjson = this.getJSONLink(psid.substring(0, psid.length -1) + 'R', child.resourceId, lowerRight, upperLeft, pDocker, cDocker);		
			}else{
				// center
				upperLeft.x = parent.bounds.upperLeft.x + (getNodeWidth(parent.stencil.id) / 2);
				upperLeft.y = parent.bounds.lowerRight.y 
				lowerRight.x = child.bounds.upperLeft.x + (getNodeWidth(child.stencil.id) / 2);
				lowerRight.y = child.bounds.upperLeft.y;
				pDocker.x = getNodeWidth(parent.stencil.id) / 2;
				pDocker.y = getNodeHeight(parent.stencil.id);
				cDocker.x = getNodeWidth(child.stencil.id) / 2;
				cDocker.y = 0;
				linkjson = this.getJSONLink(psid.substring(0, psid.length -1) + 'C', child.resourceId, lowerRight, upperLeft, pDocker, cDocker);		
			}	
					
			json.childShapes[childIndex + 1] = linkjson;
			return 	childIndex;
		}						
	}, 
	
    generateJSON: function (ruleURL, pNode, direction) {
		var modelUrl;
		if( typeof ruleURL !== 'string'){
			modelUrl = this.formulateAjaxCallURL(document.URL);			
		} else {				
			modelUrl = this.formulateAjaxCallURL(ruleURL);							
		}
		new Ajax.Request(modelUrl, {
			method: 'GET',
			onSuccess: function (transport) {
				var childConfig = Ext.decode(transport.responseText);
				var ruleData = {
					'ruleName': childConfig.childShapes[0].stencil.id,
					'ruleProperties': childConfig.childShapes[0].properties
				};
				var parentNode = this.addToJSON(childConfig, pNode, direction);
				var lHSLink = ruleData.ruleProperties['subprocesslinkl'] || '';
				var rHSLink = ruleData.ruleProperties['subprocesslinkr'] || '';
				var cLink = ruleData.ruleProperties['subprocesslinkc'] || '';
				
				if (lHSLink !== '') {
					this.generateJSON(lHSLink, parentNode, 'L');
				}			
				
				if ( rHSLink !== '') {
					this.generateJSON(rHSLink, parentNode, 'R');
				}
				
				if (  cLink !== '') {
					this.generateJSON(cLink, parentNode, 'C');
				}	
			}.bind(this),
			onFailure: function (transport) {
				Ext.Msg.alert('Connection Error', 'Cannot connect to backend');				
			}.bind(this)
		});	
    }, 
	
	showModel: function(){
		this.generateJSON(this);
		window.setTimeout((function(){                              
			this.saveSynchronously();            
        }).bind(this), 3000);	
	}
});