package customHandlers;

import java.util.Date;

/**
 * 
 * @author Mostafa
 * 
 */
public class XMLFormatter {

	private static XMLFormatter formatter = new XMLFormatter();

	private XMLFormatter() {
	}

	public static XMLFormatter getInstance() {
		return formatter;
	}

	public String formatXML(EventInfo info) {
		StringBuilder builder = new StringBuilder();
		builder.append("<eventInfo>");
		// event type
		builder.append("<eventType>");
		builder.append(info.getEventType());
		builder.append("</eventType>");
		// event description 
		builder.append("<eventDescription>");
		builder.append(info.getEventDescription());
		builder.append("</eventDescription>");
		// event date
		builder.append("<eventDate>");
		Date now = new Date();
		builder.append(now.toString());
		builder.append("</eventDate>");
		// event data
		builder.append("<eventData>");
		// activity name
		builder.append("<activityName>");
		builder.append(info.getActivityName());
		builder.append("</activityName>");
		// activity id
		builder.append("<activityId>");
		builder.append(info.getActivityId());
		builder.append("</activityId>");
		// activity type
		builder.append("<activityType>");
		builder.append(info.getActivityType());
		builder.append("</activityType>");
		// process definition
		builder.append("<processDefinition>");
		builder.append(info.getProcessDefinationName());
		builder.append("</processDefinition>");
		// process instance id
		builder.append("<processInstanceId>");
		builder.append(info.getProcessInstanceId());
		builder.append("</processInstanceId>");
		// assignee
		if (info.getTaskOwner() != null) {
			builder.append("<assignee>");
			builder.append(info.getTaskOwner());
			builder.append("</assignee>");
		}
		builder.append("</eventData>");
		builder.append("</eventInfo>");
		return builder.toString();
	}

}
