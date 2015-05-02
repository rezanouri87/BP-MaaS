package customHandlers.wfccommunicator;

import java.rmi.RemoteException;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

import org.datacontract.schemas._2004._07.CEP_Common_Events.ProcessEvent;
import org.datacontract.schemas._2004._07.CEP_Common_Events.RawEvent;
import org.datacontract.schemas._2004._07.CEP_Common_Events.TaskEvent;
import org.tempuri.IEventReceiverServiceProxy;

import customHandlers.pojo.EventInfo;


@Path("/logger")
public class EventEmitter {

	@POST
	@Consumes("text/plain")
	public static Response logActivitiEvent(String xmlEvent) {
		try {
			EventInfo event = XMLManager.generateXMLFromString(xmlEvent);
			RawEvent rawEvent = EventsMapper.MapEvent(event);
			if (rawEvent != null) {
				if(rawEvent instanceof TaskEvent ){
					System.out.println("Task new event " + ((TaskEvent)rawEvent).getName());
					System.out.println("Task new event " + ((TaskEvent)rawEvent).getProcessID());
					System.out.println("Task new event " + ((TaskEvent)rawEvent).getTask());
					System.out.println("Task new event " + ((TaskEvent)rawEvent).getTaskActor());
					System.out.println("Task new event " + ((TaskEvent)rawEvent).getTaskID());
				}else if(rawEvent instanceof ProcessEvent){
					System.out.println("Process new event " + ((ProcessEvent) rawEvent).getName());
					System.out.println("Process new event " + ((ProcessEvent) rawEvent).getProcessID());
				}
				
				IEventReceiverServiceProxy proxy = new IEventReceiverServiceProxy();
				proxy.sendEvent(rawEvent);
			}
		} catch (RemoteException e) {
			System.out.println("%%%%%%%%%%%%%%%%%%%% " + e.getMessage());
			return Response.status(500).build();
		}

		return Response.status(200).build();
	}
	
	
	@GET 
	@Produces(MediaType.TEXT_PLAIN)
	public String testWS(){
		return "7mada";
	}
	
	public static void main(String[] args) {
		String x = "<eventInfo><eventType>TASK_CREATED</eventType><eventDescription>A task has been created</eventDescription><eventDate>Mon Jan 05 14:01:08 AST 2015</eventDate><eventData><activityName>Approval task</activityName><activityType>userTask</activityType><processDefinition>simpleApprovalProcess</processDefinition><processInstanceId>1457</processInstanceId><assignee>kermit</assignee></eventData></eventInfo>";
		EventEmitter.logActivitiEvent(x);
	}
}
