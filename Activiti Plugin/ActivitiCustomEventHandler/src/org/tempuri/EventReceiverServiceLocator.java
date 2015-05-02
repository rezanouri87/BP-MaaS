/**
 * EventReceiverServiceLocator.java
 *
 * This file was auto-generated from WSDL
 * by the Apache Axis 1.4 Apr 22, 2006 (06:55:48 PDT) WSDL2Java emitter.
 */

package org.tempuri;

public class EventReceiverServiceLocator extends org.apache.axis.client.Service implements org.tempuri.EventReceiverService {

    public EventReceiverServiceLocator() {
    }


    public EventReceiverServiceLocator(org.apache.axis.EngineConfiguration config) {
        super(config);
    }

    public EventReceiverServiceLocator(java.lang.String wsdlLoc, javax.xml.namespace.QName sName) throws javax.xml.rpc.ServiceException {
        super(wsdlLoc, sName);
    }

    // Use to get a proxy class for BasicHttpBinding_IEventReceiverService
    private java.lang.String BasicHttpBinding_IEventReceiverService_address = "http://localhost:8733/Design_Time_Addresses/CEP/EventReceiver/EventReceiver";

    public java.lang.String getBasicHttpBinding_IEventReceiverServiceAddress() {
        return BasicHttpBinding_IEventReceiverService_address;
    }

    // The WSDD service name defaults to the port name.
    private java.lang.String BasicHttpBinding_IEventReceiverServiceWSDDServiceName = "BasicHttpBinding_IEventReceiverService";

    public java.lang.String getBasicHttpBinding_IEventReceiverServiceWSDDServiceName() {
        return BasicHttpBinding_IEventReceiverServiceWSDDServiceName;
    }

    public void setBasicHttpBinding_IEventReceiverServiceWSDDServiceName(java.lang.String name) {
        BasicHttpBinding_IEventReceiverServiceWSDDServiceName = name;
    }

    public org.tempuri.IEventReceiverService getBasicHttpBinding_IEventReceiverService() throws javax.xml.rpc.ServiceException {
       java.net.URL endpoint;
        try {
            endpoint = new java.net.URL(BasicHttpBinding_IEventReceiverService_address);
        }
        catch (java.net.MalformedURLException e) {
            throw new javax.xml.rpc.ServiceException(e);
        }
        return getBasicHttpBinding_IEventReceiverService(endpoint);
    }

    public org.tempuri.IEventReceiverService getBasicHttpBinding_IEventReceiverService(java.net.URL portAddress) throws javax.xml.rpc.ServiceException {
        try {
            org.tempuri.BasicHttpBinding_IEventReceiverServiceStub _stub = new org.tempuri.BasicHttpBinding_IEventReceiverServiceStub(portAddress, this);
            _stub.setPortName(getBasicHttpBinding_IEventReceiverServiceWSDDServiceName());
            return _stub;
        }
        catch (org.apache.axis.AxisFault e) {
            return null;
        }
    }

    public void setBasicHttpBinding_IEventReceiverServiceEndpointAddress(java.lang.String address) {
        BasicHttpBinding_IEventReceiverService_address = address;
    }

    /**
     * For the given interface, get the stub implementation.
     * If this service has no port for the given interface,
     * then ServiceException is thrown.
     */
    public java.rmi.Remote getPort(Class serviceEndpointInterface) throws javax.xml.rpc.ServiceException {
        try {
            if (org.tempuri.IEventReceiverService.class.isAssignableFrom(serviceEndpointInterface)) {
                org.tempuri.BasicHttpBinding_IEventReceiverServiceStub _stub = new org.tempuri.BasicHttpBinding_IEventReceiverServiceStub(new java.net.URL(BasicHttpBinding_IEventReceiverService_address), this);
                _stub.setPortName(getBasicHttpBinding_IEventReceiverServiceWSDDServiceName());
                return _stub;
            }
        }
        catch (java.lang.Throwable t) {
            throw new javax.xml.rpc.ServiceException(t);
        }
        throw new javax.xml.rpc.ServiceException("There is no stub implementation for the interface:  " + (serviceEndpointInterface == null ? "null" : serviceEndpointInterface.getName()));
    }

    /**
     * For the given interface, get the stub implementation.
     * If this service has no port for the given interface,
     * then ServiceException is thrown.
     */
    public java.rmi.Remote getPort(javax.xml.namespace.QName portName, Class serviceEndpointInterface) throws javax.xml.rpc.ServiceException {
        if (portName == null) {
            return getPort(serviceEndpointInterface);
        }
        java.lang.String inputPortName = portName.getLocalPart();
        if ("BasicHttpBinding_IEventReceiverService".equals(inputPortName)) {
            return getBasicHttpBinding_IEventReceiverService();
        }
        else  {
            java.rmi.Remote _stub = getPort(serviceEndpointInterface);
            ((org.apache.axis.client.Stub) _stub).setPortName(portName);
            return _stub;
        }
    }

    public javax.xml.namespace.QName getServiceName() {
        return new javax.xml.namespace.QName("http://tempuri.org/", "EventReceiverService");
    }

    private java.util.HashSet ports = null;

    public java.util.Iterator getPorts() {
        if (ports == null) {
            ports = new java.util.HashSet();
            ports.add(new javax.xml.namespace.QName("http://tempuri.org/", "BasicHttpBinding_IEventReceiverService"));
        }
        return ports.iterator();
    }

    /**
    * Set the endpoint address for the specified port name.
    */
    public void setEndpointAddress(java.lang.String portName, java.lang.String address) throws javax.xml.rpc.ServiceException {
        
if ("BasicHttpBinding_IEventReceiverService".equals(portName)) {
            setBasicHttpBinding_IEventReceiverServiceEndpointAddress(address);
        }
        else 
{ // Unknown Port Name
            throw new javax.xml.rpc.ServiceException(" Cannot set Endpoint Address for Unknown Port" + portName);
        }
    }

    /**
    * Set the endpoint address for the specified port name.
    */
    public void setEndpointAddress(javax.xml.namespace.QName portName, java.lang.String address) throws javax.xml.rpc.ServiceException {
        setEndpointAddress(portName.getLocalPart(), address);
    }

}
