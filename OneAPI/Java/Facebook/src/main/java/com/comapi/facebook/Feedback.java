package com.comapi.facebook;

/**
 * Class that represents feedback to be displayed in a page after a service call.
 * @author dave.baddeley
 */
public class Feedback {
    private Boolean succeeded;
    private String message;

    public Feedback()
    {
        succeeded = false;
        message = "";
    }
    
    /**
     * @return the Succeeded
     */
    public Boolean getSucceeded() {
        return succeeded;
    }

    /**
     * @param succeeded the Succeeded to set
     */
    public void setSucceeded(Boolean Succeeded) {
        this.succeeded = Succeeded;
    }

    /**
     * @return the Message
     */
    public String getMessage() {
        return message;
    }

    /**
     * @param Message the Message to set
     */
    public void setMessage(String Message) {
        this.message = Message;
    }
    
}
