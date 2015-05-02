package customHandlers.pojo;

public class EventData {

	private String activityName;

	private String activityId;

	private String activityType;

	private String processDefinition;

	private String processInstanceId;

	private String assignee;

	public EventData() {
	}

	public EventData(String activityName, String activityId,
			String activityType, String processDefinition,
			String processInstanceId, String assignee) {
		super();
		this.activityName = activityName;
		this.activityId = activityId;
		this.activityType = activityType;
		this.processDefinition = processDefinition;
		this.processInstanceId = processInstanceId;
		this.assignee = assignee;
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

	public String getProcessDefinition() {
		return processDefinition;
	}

	public void setProcessDefinition(String processDefinition) {
		this.processDefinition = processDefinition;
	}

	public String getProcessInstanceId() {
		return processInstanceId;
	}

	public void setProcessInstanceId(String processInstanceId) {
		this.processInstanceId = processInstanceId;
	}

	public String getAssignee() {
		return assignee;
	}

	public void setAssignee(String assignee) {
		this.assignee = assignee;
	}

	public String getActivityId() {
		return activityId;
	}

	public void setActivityId(String activityId) {
		this.activityId = activityId;
	}

}
