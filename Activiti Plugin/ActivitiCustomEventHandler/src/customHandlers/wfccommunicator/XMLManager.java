package customHandlers.wfccommunicator;

import java.io.ByteArrayInputStream;
import java.io.InputStream;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

import customHandlers.pojo.EventInfo;


public class XMLManager {
	
	public static EventInfo generateXMLFromString(String xmlString){
		try {
			JAXBContext context = JAXBContext.newInstance(EventInfo.class);
			Unmarshaller un = context.createUnmarshaller();
			InputStream is = new ByteArrayInputStream(xmlString.getBytes());
			return (EventInfo) un.unmarshal(is);
			
		} catch (JAXBException e) {
			e.printStackTrace();
		}
		return new EventInfo() ;
	}
	
}
