package customHandlers.wfccommunicator;

import org.datacontract.schemas._2004._07.CEP_Common_Events.ProcessComplete;
import org.datacontract.schemas._2004._07.CEP_Common_Events.ProcessStart;
import org.datacontract.schemas._2004._07.CEP_Common_Events.RawEvent;
import org.datacontract.schemas._2004._07.CEP_Common_Events.TaskComplete;
import org.datacontract.schemas._2004._07.CEP_Common_Events.TaskStart;
import org.datacontract.schemas._2004._07.CEP_Common_Events.TaskSuspend;

import customHandlers.pojo.EventInfo;


public class EventsMapper {

	private static String getActor(EventInfo event) {
		return event.getEventData().getAssignee() == null ? "" : event.getEventData().getAssignee();
	}

	public static RawEvent MapEvent(EventInfo event) {
		RawEvent newEvent = null;
//		if ("TASK_STARTED".equals(event.getEventType())) {
//			newEvent = new TaskStart();
//			((TaskStart) newEvent).setName(event.getEventData().getActivityName());
//			((TaskStart) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
//			((TaskStart) newEvent).setTaskID(event.getEventData().getActivityId());
//			((TaskStart) newEvent).setTaskActor(getActor(event));
//			((TaskStart) newEvent).setTask(event.getEventType());
//		} else 
		if ("PROCESS_STARTED".equals(event.getEventType())) {
			newEvent = new ProcessStart();
			((ProcessStart) newEvent).setName(event.getEventData().getProcessDefinition());
			((ProcessStart) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
		} else if ("TASK_CREATED".equals(event.getEventType())) {
			newEvent = new TaskStart();
			((TaskStart) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
			((TaskStart) newEvent).setTaskID(event.getEventData().getActivityId());
			((TaskStart) newEvent).setTaskActor(getActor(event));
			((TaskStart) newEvent).setTask(event.getEventData().getActivityName());
		} else if ("TASK_TIMEOUT".equals(event.getEventType())) {
			newEvent = new TaskSuspend();
			((TaskSuspend) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
			((TaskSuspend) newEvent).setTaskID(event.getEventData().getActivityId());
			((TaskSuspend) newEvent).setTaskActor(getActor(event));
			((TaskSuspend) newEvent).setTask(event.getEventData().getActivityName());
		} else if ("ACTIVITY_TIMEOUT".equals(event.getEventType())) {
			newEvent = new TaskSuspend();
			((TaskSuspend) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
			((TaskSuspend) newEvent).setTaskID(event.getEventData().getActivityId());
			((TaskSuspend) newEvent).setTaskActor(getActor(event));
			((TaskSuspend) newEvent).setTask(event.getEventData().getActivityName());
		} else if ("TASK_COMPLETED".equals(event.getEventType())) {
			newEvent = new TaskComplete();
			((TaskComplete) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
			((TaskComplete) newEvent).setTaskID(event.getEventData().getActivityId());
			((TaskComplete) newEvent).setTaskActor(getActor(event));
			((TaskComplete) newEvent).setTask(event.getEventData().getActivityName());
		} else if ("PROCESS_COMPLETED".equals(event.getEventType())) {
			newEvent = new ProcessComplete();
			((ProcessComplete) newEvent).setName(event.getEventData().getProcessDefinition());
			((ProcessComplete) newEvent).setProcessID(Integer.parseInt(event.getEventData().getProcessInstanceId()));
		}
		return newEvent;
	}
}
