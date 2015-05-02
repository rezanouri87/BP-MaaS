package customHandlers;

import org.activiti.engine.delegate.event.ActivitiEvent;
import org.activiti.engine.delegate.event.ActivitiEventListener;
import org.activiti.engine.delegate.event.impl.ActivitiActivityEventImpl;
import org.activiti.engine.delegate.event.impl.ActivitiEntityEventImpl;
import org.activiti.engine.delegate.event.impl.ActivitiErrorEventImpl;
import org.activiti.engine.delegate.event.impl.ActivitiMessageEventImpl;
import org.activiti.engine.delegate.event.impl.ActivitiSignalEventImpl;
import org.activiti.engine.impl.persistence.entity.TaskEntity;

public class MyEventListener implements ActivitiEventListener {

	private static PropertiesLoader loader = PropertiesLoader.getInstance();

	@Override
	public void onEvent(ActivitiEvent event) {
		EventInfo info = new EventInfo();
		TaskEntity entity;
		if (loader.getStartListeningTime() <= System.currentTimeMillis()) {

			switch (event.getType()) {

			// Task Started, process started
			case ACTIVITY_STARTED:
				ActivitiActivityEventImpl asEvent = (ActivitiActivityEventImpl) event;
				if (asEvent.getActivityType().equals(PropertiesLoader.TASK_EVENT_TYPE)) {
					// task
					info.setEventType("TASK_STARTED");
					info.setEventDescription(loader.getPropertyValue("TASK_STARTED"));
					info.setActivityType(asEvent.getActivityType());
					info.setActivityName(asEvent.getActivityName());
					info.setActivityId(asEvent.getActivityId());
				} else if (asEvent.getActivityType().equals(PropertiesLoader.START_EVENT_TYPE)) {
					// start process
					info.setEventType("PROCESS_STARTED");
					info.setEventDescription(loader.getPropertyValue("PROCESS_STARTED"));
					info.setActivityType("Process");
					info.setActivityName(asEvent.getActivityName());
					info.setActivityId(asEvent.getProcessInstanceId());
				} else {
					// other activity
					info.setEventType("ACTIVITY_STARTED");
					info.setEventDescription(loader.getPropertyValue("ACTIVITY_STARTED"));
					info.setActivityType(asEvent.getActivityType());
					info.setActivityName(asEvent.getActivityName());
					info.setActivityId(asEvent.getActivityId());
				}
				info.setProcessDefinationName(asEvent.getProcessDefinitionId());
				info.setProcessInstanceId(asEvent.getProcessInstanceId());
				break;

			// Task Created
			case TASK_CREATED:
				ActivitiEntityEventImpl tcrEvent = (ActivitiEntityEventImpl) event;
				entity = (TaskEntity) tcrEvent.getEntity();
				info.setEventType("TASK_CREATED");
				info.setEventDescription(loader.getPropertyValue("TASK_CREATED"));
				info.setActivityName(entity.getName());
				info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
				info.setTaskOwner(entity.getAssignee());
				info.setProcessDefinationName(tcrEvent.getProcessDefinitionId());
				info.setProcessInstanceId(tcrEvent.getProcessInstanceId());
				info.setActivityId(entity.getId());
				break;

			// task assigned
			case TASK_ASSIGNED:
				ActivitiEntityEventImpl taEvent = (ActivitiEntityEventImpl) event;
				entity = (TaskEntity) taEvent.getEntity();
				info.setEventType("TASK_ASSIGNED");
				info.setEventDescription(loader.getPropertyValue("TASK_ASSIGNED"));
				info.setActivityName(entity.getName());
				info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
				info.setTaskOwner(entity.getAssignee());
				info.setProcessDefinationName(taEvent.getProcessDefinitionId());
				info.setProcessInstanceId(taEvent.getProcessInstanceId());
				info.setActivityId(entity.getId());
				break;

			// task deleted
			case ENTITY_DELETED:
				ActivitiEntityEventImpl edEvent = (ActivitiEntityEventImpl) event;
				if (edEvent.getEntity() instanceof TaskEntity) {
					entity = (TaskEntity) edEvent.getEntity();
					info.setEventType("TASK_DELETED");
					info.setEventDescription(loader.getPropertyValue("TASK_DELETED"));
					info.setActivityName(entity.getName());
					info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
					info.setProcessDefinationName(edEvent.getProcessDefinitionId());
					info.setProcessInstanceId(edEvent.getProcessInstanceId());
					info.setActivityId(entity.getId());
				}
				break;
			// task timeout
			case ACTIVITY_TIMEOUT:
				ActivitiActivityEventImpl atEvent = (ActivitiActivityEventImpl) event;
				if (atEvent.getActivityType().equals(
				// user task
						PropertiesLoader.TASK_EVENT_TYPE)) {
					info.setEventType("TASK_TIMEOUT");
					info.setEventDescription(loader.getPropertyValue("TASK_TIMEOUT"));
					info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
				} else {
					// any other activity
					info.setEventType("ACTIVITY_TIMEOUT");
					info.setEventDescription(loader.getPropertyValue("ACTIVITY_TIMEOUT"));
					info.setActivityType(atEvent.getActivityType());
				}
				info.setActivityId(atEvent.getActivityId());
				info.setActivityName(atEvent.getActivityName());
				info.setProcessDefinationName(atEvent.getProcessDefinitionId());
				info.setProcessInstanceId(atEvent.getProcessInstanceId());
				break;

			// task signaled
			case ACTIVITY_SIGNALED:
				ActivitiSignalEventImpl asgEvent = (ActivitiSignalEventImpl) event;
				if (asgEvent.getActivityType().equals(
				// user task
						PropertiesLoader.TASK_EVENT_TYPE)) {
					info.setEventType("TASK_SIGNALED");
					info.setEventDescription(loader.getPropertyValue("TASK_SIGNALED"));
					info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
				} else {
					// any other activity
					info.setEventType("ACTIVITY_SIGNALED");
					info.setEventDescription(loader.getPropertyValue("ACTIVITY_SIGNALED"));
					info.setActivityType(asgEvent.getActivityType());
				}
				info.setActivityId(asgEvent.getActivityId());
				info.setActivityName(asgEvent.getActivityName());
				info.setProcessDefinationName(asgEvent.getProcessDefinitionId());
				info.setProcessInstanceId(asgEvent.getProcessInstanceId());
				break;
			// task completed
			case TASK_COMPLETED:
				ActivitiEntityEventImpl tcEvent = (ActivitiEntityEventImpl) event;
				TaskEntity tskEntity = (TaskEntity) tcEvent.getEntity();
				info.setEventType("TASK_COMPLETED");
				info.setEventDescription(loader.getPropertyValue("TASK_COMPLETED"));
				info.setActivityName(tskEntity.getName());
				info.setActivityType(PropertiesLoader.TASK_EVENT_TYPE);
				info.setTaskOwner(tskEntity.getAssignee());
				info.setProcessDefinationName(tcEvent.getProcessDefinitionId());
				info.setProcessInstanceId(tcEvent.getProcessInstanceId());
				info.setActivityId(tskEntity.getId());
				break;

			// activity completed
			case ACTIVITY_COMPLETED:
				ActivitiActivityEventImpl acEvent = (ActivitiActivityEventImpl) event;
				if (!acEvent.getActivityType().equals(PropertiesLoader.TASK_EVENT_TYPE)) {
					info.setEventType("ACTIVITY_COMPLETED");
					info.setEventDescription(loader.getPropertyValue("ACTIVITY_COMPLETED"));
					info.setActivityName(acEvent.getActivityName());
					info.setActivityType(acEvent.getActivityType());
					info.setProcessDefinationName(acEvent.getProcessDefinitionId());
					info.setProcessInstanceId(acEvent.getProcessInstanceId());
					info.setActivityId(acEvent.getActivityId());
				}
				break;

			// extra
			case ACTIVITY_MESSAGE_RECEIVED:
				ActivitiMessageEventImpl amsgEvent = (ActivitiMessageEventImpl) event;
				info.setEventType("ACTIVITY_MESSAGE_RECEIVED");
				info.setEventDescription(loader.getPropertyValue("ACTIVITY_MESSAGE_RECEIVED"));
				info.setActivityName(amsgEvent.getActivityName());
				info.setActivityType(amsgEvent.getActivityType());
				info.setProcessDefinationName(amsgEvent.getProcessDefinitionId());
				info.setProcessInstanceId(amsgEvent.getProcessInstanceId());
				info.setActivityId(amsgEvent.getActivityId());
				break;

			// extra
			case ACTIVITY_ERROR_RECEIVED:
				ActivitiErrorEventImpl aerrEvent = (ActivitiErrorEventImpl) event;
				info.setEventType("ACTIVITY_ERROR_RECEIVED");
				info.setEventDescription(loader.getPropertyValue("ACTIVITY_ERROR_RECEIVED"));
				info.setActivityName(aerrEvent.getActivityName());
				info.setActivityType(aerrEvent.getActivityType());
				info.setProcessDefinationName(aerrEvent.getProcessDefinitionId());
				info.setProcessInstanceId(aerrEvent.getProcessInstanceId());
				info.setActivityId(aerrEvent.getActivityId());
				break;

			// extra
			case UNCAUGHT_BPMN_ERROR:
				ActivitiErrorEventImpl berrEvent = (ActivitiErrorEventImpl) event;
				info.setEventType("UNCAUGHT_BPMN_ERROR");
				info.setEventDescription(loader.getPropertyValue("UNCAUGHT_BPMN_ERROR"));
				info.setActivityName(berrEvent.getActivityName());
				info.setActivityType(berrEvent.getActivityType());
				info.setProcessDefinationName(berrEvent.getProcessDefinitionId());
				info.setProcessInstanceId(berrEvent.getProcessInstanceId());
				info.setActivityId(berrEvent.getActivityId());
				break;

			// process completed
			case PROCESS_COMPLETED:
				ActivitiEntityEventImpl pcEvent = (ActivitiEntityEventImpl) event;
				info.setEventType("PROCESS_COMPLETED");
				info.setEventDescription(loader.getPropertyValue("PROCESS_COMPLETED"));
				info.setProcessDefinationName(pcEvent.getProcessDefinitionId());
				info.setProcessInstanceId(pcEvent.getProcessInstanceId());
				info.setActivityId(pcEvent.getProcessInstanceId());
				break;

			default:
			}
			if (info.getEventType() != null) {
				String xmlEventData = XMLFormatter.getInstance().formatXML(info);
				System.out.println("************** Event About to sent: " + info.getEventType());
				WebServiceCommunicator.getInstance().callWebservice(xmlEventData);
			}
		}

	}

	@Override
	public boolean isFailOnException() {
		// The logic in the onEvent method of this listener is not critical,
		// exceptions
		// can be ignored if logging fails...
		return false;
	}
}
