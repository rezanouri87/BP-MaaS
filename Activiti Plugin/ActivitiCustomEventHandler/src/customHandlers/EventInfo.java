package customHandlers;

public class EventInfo {
	private String eventType;
	private String eventDescription;
	private String processDefinationName;
	private String processInstanceId;
	private String activityName;
	private String activityId;
	private String activityType;
	private String taskOwner;

	public EventInfo() {
	}

	public String getEventType() {
		return eventType;
	}

	public void setEventType(String eventType) {
		this.eventType = eventType;
	}

	public String getEventDescription() {
		return eventDescription;
	}

	public void setEventDescription(String eventDescription) {
		this.eventDescription = eventDescription;
	}

	public String getProcessDefinationName() {
		return processDefinationName.split(":")[0];
	}

	public void setProcessDefinationName(String processDefinationName) {
		this.processDefinationName = processDefinationName;
	}

	public String getProcessInstanceId() {
		return processInstanceId;
	}

	public void setProcessInstanceId(String processInstanceId) {
		this.processInstanceId = processInstanceId;
	}

	public String getActivityName() {
		return activityName;
	}

	public void setActivityName(String activityName) {
		this.activityName = activityName;
	}

	public String getActivityType() {
		return activityType;
	}

	public void setActivityType(String activityType) {
		this.activityType = activityType;
	}

	public String getTaskOwner() {
		return taskOwner;
	}

	public void setTaskOwner(String taskOwner) {
		this.taskOwner = taskOwner;
	}

	public String getActivityId() {
		return activityId;
	}

	public void setActivityId(String activityId) {
		this.activityId = activityId;
	}

	@Override
	public String toString() {
		return eventType + " : " + eventDescription + " : " + activityName
				+ " : " + activityType + " : " + processDefinationName + " : "
				+ processInstanceId + " : " + taskOwner;
	}
}
