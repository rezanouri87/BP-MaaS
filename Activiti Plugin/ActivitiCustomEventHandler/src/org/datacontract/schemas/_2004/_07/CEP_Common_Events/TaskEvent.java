/**
 * TaskEvent.java
 *
 * This file was auto-generated from WSDL
 * by the Apache Axis 1.4 Apr 22, 2006 (06:55:48 PDT) WSDL2Java emitter.
 */

package org.datacontract.schemas._2004._07.CEP_Common_Events;

public class TaskEvent  extends org.datacontract.schemas._2004._07.CEP_Common_Events.RawEvent  implements java.io.Serializable {
    private java.lang.Object processID;

    private java.lang.Object task;

    private java.lang.Object taskActor;

    private java.lang.Object taskID;

    public TaskEvent() {
    }

    public TaskEvent(
           java.lang.String name,
           java.lang.Object processID,
           java.lang.Object task,
           java.lang.Object taskActor,
           java.lang.Object taskID) {
        super(
            name);
        this.processID = processID;
        this.task = task;
        this.taskActor = taskActor;
        this.taskID = taskID;
    }


    /**
     * Gets the processID value for this TaskEvent.
     * 
     * @return processID
     */
    public java.lang.Object getProcessID() {
        return processID;
    }


    /**
     * Sets the processID value for this TaskEvent.
     * 
     * @param processID
     */
    public void setProcessID(java.lang.Object processID) {
        this.processID = processID;
    }


    /**
     * Gets the task value for this TaskEvent.
     * 
     * @return task
     */
    public java.lang.Object getTask() {
        return task;
    }


    /**
     * Sets the task value for this TaskEvent.
     * 
     * @param task
     */
    public void setTask(java.lang.Object task) {
        this.task = task;
    }


    /**
     * Gets the taskActor value for this TaskEvent.
     * 
     * @return taskActor
     */
    public java.lang.Object getTaskActor() {
        return taskActor;
    }


    /**
     * Sets the taskActor value for this TaskEvent.
     * 
     * @param taskActor
     */
    public void setTaskActor(java.lang.Object taskActor) {
        this.taskActor = taskActor;
    }


    /**
     * Gets the taskID value for this TaskEvent.
     * 
     * @return taskID
     */
    public java.lang.Object getTaskID() {
        return taskID;
    }


    /**
     * Sets the taskID value for this TaskEvent.
     * 
     * @param taskID
     */
    public void setTaskID(java.lang.Object taskID) {
        this.taskID = taskID;
    }

    private java.lang.Object __equalsCalc = null;
    public synchronized boolean equals(java.lang.Object obj) {
        if (!(obj instanceof TaskEvent)) return false;
        TaskEvent other = (TaskEvent) obj;
        if (obj == null) return false;
        if (this == obj) return true;
        if (__equalsCalc != null) {
            return (__equalsCalc == obj);
        }
        __equalsCalc = obj;
        boolean _equals;
        _equals = super.equals(obj) && 
            ((this.processID==null && other.getProcessID()==null) || 
             (this.processID!=null &&
              this.processID.equals(other.getProcessID()))) &&
            ((this.task==null && other.getTask()==null) || 
             (this.task!=null &&
              this.task.equals(other.getTask()))) &&
            ((this.taskActor==null && other.getTaskActor()==null) || 
             (this.taskActor!=null &&
              this.taskActor.equals(other.getTaskActor()))) &&
            ((this.taskID==null && other.getTaskID()==null) || 
             (this.taskID!=null &&
              this.taskID.equals(other.getTaskID())));
        __equalsCalc = null;
        return _equals;
    }

    private boolean __hashCodeCalc = false;
    public synchronized int hashCode() {
        if (__hashCodeCalc) {
            return 0;
        }
        __hashCodeCalc = true;
        int _hashCode = super.hashCode();
        if (getProcessID() != null) {
            _hashCode += getProcessID().hashCode();
        }
        if (getTask() != null) {
            _hashCode += getTask().hashCode();
        }
        if (getTaskActor() != null) {
            _hashCode += getTaskActor().hashCode();
        }
        if (getTaskID() != null) {
            _hashCode += getTaskID().hashCode();
        }
        __hashCodeCalc = false;
        return _hashCode;
    }

    // Type metadata
    private static org.apache.axis.description.TypeDesc typeDesc =
        new org.apache.axis.description.TypeDesc(TaskEvent.class, true);

    static {
        typeDesc.setXmlType(new javax.xml.namespace.QName("http://schemas.datacontract.org/2004/07/CEP.Common.Events", "TaskEvent"));
        org.apache.axis.description.ElementDesc elemField = new org.apache.axis.description.ElementDesc();
        elemField.setFieldName("processID");
        elemField.setXmlName(new javax.xml.namespace.QName("http://schemas.datacontract.org/2004/07/CEP.Common.Events", "ProcessID"));
        elemField.setXmlType(new javax.xml.namespace.QName("http://www.w3.org/2001/XMLSchema", "anyType"));
        elemField.setMinOccurs(0);
        elemField.setNillable(true);
        typeDesc.addFieldDesc(elemField);
        elemField = new org.apache.axis.description.ElementDesc();
        elemField.setFieldName("task");
        elemField.setXmlName(new javax.xml.namespace.QName("http://schemas.datacontract.org/2004/07/CEP.Common.Events", "Task"));
        elemField.setXmlType(new javax.xml.namespace.QName("http://www.w3.org/2001/XMLSchema", "string"));
        elemField.setMinOccurs(0);
        elemField.setNillable(true);
        typeDesc.addFieldDesc(elemField);
        elemField = new org.apache.axis.description.ElementDesc();
        elemField.setFieldName("taskActor");
        elemField.setXmlName(new javax.xml.namespace.QName("http://schemas.datacontract.org/2004/07/CEP.Common.Events", "TaskActor"));
        elemField.setXmlType(new javax.xml.namespace.QName("http://www.w3.org/2001/XMLSchema", "string"));
        elemField.setMinOccurs(0);
        elemField.setNillable(true);
        typeDesc.addFieldDesc(elemField);
        elemField = new org.apache.axis.description.ElementDesc();
        elemField.setFieldName("taskID");
        elemField.setXmlName(new javax.xml.namespace.QName("http://schemas.datacontract.org/2004/07/CEP.Common.Events", "TaskID"));
        elemField.setXmlType(new javax.xml.namespace.QName("http://www.w3.org/2001/XMLSchema", "string"));
        elemField.setMinOccurs(0);
        elemField.setNillable(true);
        typeDesc.addFieldDesc(elemField);
    }

    /**
     * Return type metadata object
     */
    public static org.apache.axis.description.TypeDesc getTypeDesc() {
        return typeDesc;
    }

    /**
     * Get Custom Serializer
     */
    public static org.apache.axis.encoding.Serializer getSerializer(
           java.lang.String mechType, 
           java.lang.Class _javaType,  
           javax.xml.namespace.QName _xmlType) {
        return 
          new  org.apache.axis.encoding.ser.BeanSerializer(
            _javaType, _xmlType, typeDesc);
    }

    /**
     * Get Custom Deserializer
     */
    public static org.apache.axis.encoding.Deserializer getDeserializer(
           java.lang.String mechType, 
           java.lang.Class _javaType,  
           javax.xml.namespace.QName _xmlType) {
        return 
          new  org.apache.axis.encoding.ser.BeanDeserializer(
            _javaType, _xmlType, typeDesc);
    }

}
