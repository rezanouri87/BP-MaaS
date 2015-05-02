package org.tempuri;

public class IEventReceiverServiceProxy implements org.tempuri.IEventReceiverService {
  private String _endpoint = null;
  private org.tempuri.IEventReceiverService iEventReceiverService = null;
  
  public IEventReceiverServiceProxy() {
    _initIEventReceiverServiceProxy();
  }
  
  public IEventReceiverServiceProxy(String endpoint) {
    _endpoint = endpoint;
    _initIEventReceiverServiceProxy();
  }
  
  private void _initIEventReceiverServiceProxy() {
    try {
      iEventReceiverService = (new org.tempuri.EventReceiverServiceLocator()).getBasicHttpBinding_IEventReceiverService();
      if (iEventReceiverService != null) {
        if (_endpoint != null)
          ((javax.xml.rpc.Stub)iEventReceiverService)._setProperty("javax.xml.rpc.service.endpoint.address", _endpoint);
        else
          _endpoint = (String)((javax.xml.rpc.Stub)iEventReceiverService)._getProperty("javax.xml.rpc.service.endpoint.address");
      }
      
    }
    catch (javax.xml.rpc.ServiceException serviceException) {}
  }
  
  public String getEndpoint() {
    return _endpoint;
  }
  
  public void setEndpoint(String endpoint) {
    _endpoint = endpoint;
    if (iEventReceiverService != null)
      ((javax.xml.rpc.Stub)iEventReceiverService)._setProperty("javax.xml.rpc.service.endpoint.address", _endpoint);
    
  }
  
  public org.tempuri.IEventReceiverService getIEventReceiverService() {
    if (iEventReceiverService == null)
      _initIEventReceiverServiceProxy();
    return iEventReceiverService;
  }
  
  public void sendEvent(org.datacontract.schemas._2004._07.CEP_Common_Events.RawEvent obj) throws java.rmi.RemoteException{
    if (iEventReceiverService == null)
      _initIEventReceiverServiceProxy();
    iEventReceiverService.sendEvent(obj);
  }
  
  
}