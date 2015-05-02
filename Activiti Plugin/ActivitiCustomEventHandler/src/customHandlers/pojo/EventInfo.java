package customHandlers.pojo;

import java.util.Date;

import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement(name = "eventInfo")
public class EventInfo {

	private String eventType;
	private String eventDescription;
	private Date eventDate;
	private EventData eventData;

	public EventInfo() {
	}

	public EventInfo(String eventType, String eventDescription, Date eventDate, EventData eventData) {
		super();
		this.eventType = eventType;
		this.eventDescription = eventDescription;
		this.eventDate = eventDate;
		this.eventData = eventData;
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

	public Date getEventDate() {
		return eventDate;
	}

	public void setEventDate(Date eventDate) {
		this.eventDate = eventDate;
	}

	public EventData getEventData() {
		return eventData;
	}

	public void setEventData(EventData eventData) {
		this.eventData = eventData;
	}
}
