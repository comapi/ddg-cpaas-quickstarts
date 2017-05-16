# Index page model for exchanging data with the controller
class IndexModel

    def initialize
        @metadata = nil
        @testButtonsDisplay = "none"
        @feedback = nil
    end
 
    attr_accessor :metadata
    
    attr_accessor :testButtonsDisplay
    
    attr_accessor :feedback

end

# Feedback object for the UI to display
class Feedback

    def initialize
        @succeeded = false
        @message = ""
    end

    attr_accessor :succeeded

    attr_accessor :message

end

# Used to return a web service call result
class WebserviceResult

attr_accessor :data

attr_accessor :succeeded

attr_accessor :feedback

end