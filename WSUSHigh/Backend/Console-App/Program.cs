string designatedDirectory = @"C:\WSUSUpdates";

WSUSHigh high = new WSUSHigh(designatedDirectory);
high.ReceiveUpdates();
