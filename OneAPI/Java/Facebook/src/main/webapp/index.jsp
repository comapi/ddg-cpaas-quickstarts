<%@taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%-- 
    Document   : index
    Created on : 17-May-2017, 15:30:07
    Author     : dave.baddeley
--%>

<%@page contentType="text/html" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html lang="en">

<head>
    <title>Comapi Facebook Opt-in tutorial</title>
    <link rel="shortcut icon" href="${pageContext.request.contextPath}/images/favicon.ico?" type="image/x-icon" />
    <link rel="stylesheet" href="${pageContext.request.contextPath}/css/style.css" />
    <link rel="stylesheet" href="${pageContext.request.contextPath}/css/toastr.min.css" />
    <script src="https://code.jquery.com/jquery-2.2.4.min.js" integrity="sha256-BbhdlvQf/xTY9gja0Dq3HiwQF8LaCRTXxZKRutelT44=" crossorigin="anonymous"></script>
    <script language="Javascript" src="${pageContext.request.contextPath}/scripts/toastr.min.js"></script>

    <!-- Facebook Send to Messenger standard code to include in the head of the page -->
    <script lang="Javascript">
        window.fbAsyncInit = function () {
            FB.init({
                appId: "336037380081042",
                xfbml: true,
                version: "v2.6"
            });

            FB.Event.subscribe('send_to_messenger', function (e) {
                // callback for events triggered by the Facebook plugin
                if (e.event == "opt_in") {
                    // Send to Messenger clicked render test buttons
                    $("#testButtons").show();
                }
            });
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/sdk.js";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>
</head>

<body class="container">
    <jsp:useBean id="handler" scope="session" class="com.comapi.facebook.IndexHandler" />
    

    <%
        // HTTP POST Handler code
        if (request.getParameter("SimpleTest") != null)
        {
            // Ensure the button section remains displayed
            handler.setTestButtonsDisplay("block");
            
            // Send a simple Facebook message.
            handler.SendFacebookMessage(handler.getProfileId(), "A simple text message", null);
        }
        
        if (request.getParameter("RichTest") != null)
        {
            // Ensure the button section remains displayed
            handler.setTestButtonsDisplay("block");
            
            // Send a rich Facebook message.
            String fbCustomMessage = "{"
                + "    \"fbMessenger\": {"
                + "      \"text\": \"Pick a color:\","
                + "      \"quick_replies\": ["
                + "      {"
                + "        \"content_type\": \"text\","
                + "        \"title\": \"Red\","
                + "        \"payload\": \"DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_RED\""
                + "      },"
                + "      {"
                + "        \"content_type\": \"text\","
                + "        \"title\": \"Blue\","
                + "        \"payload\": \"DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_BLUE\""
                + "      }"
                + "      ]"
                + "    }"
                + "  }";
            
            handler.SendFacebookMessage(handler.getProfileId(), "A simple text message", fbCustomMessage);
        }
    %>
    
    <header>
        <h1>Facebook Opt-in Page</h1>
    </header>

    <main>
        <p>
            You will need to add the Facebook <a href="//developers.facebook.com/docs/messenger-platform/plugin-reference/send-to-messenger"
                target="_blank"><b>Send to Messenger</b></a> web plugin to your website to get permission to send the user messages
            via Facebook. This would usually be on your check out page etc...
        </p>

        <hr>

        <p>
            Click below to receive updates via Facebook Messenger
        </p>

        <!-- The data-ref field will be populated with the metadata returned from Comapi's Facebook webservice', add your own Facebook page_id -->
        <div class="fb-send-to-messenger" 
            messenger_app_id="336037380081042" 
            page_id=">>>>YOUR FACEBOOK PAGE ID<<<<" 
            data-ref="<jsp:getProperty name="handler" property="metadata" />"
            color="blue" size="large">
        </div>

        <hr>
        <p>
            When it is clicked a unique Facebook Messenger Id will be created for the customer and your page and automatically sent to
            Comapi where a <b>fbMessengerId</b> field will be added to the profile you indicated using the <b>data-ref</b>            field on the Send to Messenger control
        </p>
        <div id="testButtons" style="display: <jsp:getProperty name="handler" property="testButtonsDisplay" />;">
            <hr>
            <p>Use the test buttons below to send test messages to Facebook</p>
            <form id="buttonForm" method="POST">
                
                <button name="SimpleTest">Send Simple Test Message</button>
                <button name="RichTest">Send Rich Test Message</button>
            </form>
            <hr>
        </div>
            <c:if test="handler.getFeedback() != null" var="result">
                <script language="Javascript">
                    // Setup Toastr
                    toastr.options.closeButton = true;
                    toastr.options.closeMethod = 'fadeOut';
                    toastr.options.closeDuration = 300;
                    toastr.options.closeEasing = 'swing';
                    toastr.options.progressBar = true;

                    <c:choose>
                        <c:when test="handler.getFeedback().getSucceeded()">
                            toastr.success("<%= handler.getFeedback().getMessage() %>", "Test message sent");
                        </c:when>
                        <c:otherwise>
                            toastr.error("<%= handler.getFeedback().getMessage() %>", "Test message failed");
                        </c:otherwise>
                    </c:choose>
                </script>
            </c:if>
 

    </main>

</body>

</html>
