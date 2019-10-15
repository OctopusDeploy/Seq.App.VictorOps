# Seq.App.VictorOps

## Installation 

Installation is done via a Nuget Feed within Seq, you will also need to enable the `REST integration` in VictorOps

## Configuration

Suggest turning on event streaming and picking a signal that only includes the messages you want to be notified about

  **VictorOps Url**
  The alert url for creating incidents, probably should be `https://alert.victorops.com/integrations/generic/20131114/alert/`

  **Incident Title**
  Just the title of the incident

  **REST API Key**
  After enabling the REST integration it is the value before the `Routing Key`

  **Routing Key** _(optional)_
  The VictorOps routing key, if none is provided it will go to the default routing policy

  **Test Mode** 
  Enable this to allow you to test the integration without sending notifications. You can use the logs for the Seq App instance to determine what is going to happen

  