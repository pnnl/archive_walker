pisdk = NET.addAssembly('OSIsoft.PISDK')
pisdksrv = NET.addAssembly('OSIsoft.PITimeServer')
pisdkcom = NET.addAssembly('OSIsoft.PISDKCommon')
import PISDK.*

pi_sdk =  PISDK.PISDKClass();

pi_point1 = pi_sdk.GetPoint('\\Eioc-pi.pnl.gov\DemoBA:Frequency');
pi_point2 = pi_sdk.GetPoint('\\Eioc-pi.pnl.gov\DemoBA:TotalWind');

 
time_start = System.String('2-Jan-2015 10:00:00'); 
time_end = System.String('2-Jan-2015 10:05:00');  
pi_data1 = pi_point1.Data.RecordedValues(time_start,time_end);  
pi_data2 = pi_point2.Data.RecordedValues(time_start,time_end);  
 
 
freq=zeros(2,pi_data1.Count);
wind=zeros(2,pi_data2.Count);

 
for i = 1:pi_data1.Count()
  %pv = pi_data.Item(i);
  %disp(sprintf('%f %f',pv.Value,pv.TimeStamp.UTCSeconds))
freq(2,i)=pi_data1.Item(i).Value;
freq(1,i)=pi_data1.Item(i).TimeStamp.UTCSeconds;
end

 
for i = 1:pi_data2.Count() 
 wind(2,i)=pi_data2.Item(i).Value;
wind(1,i)=pi_data2.Item(i).TimeStamp.UTCSeconds;
end

figure(1)
plot(freq(2,:));

figure(2)
plot(wind(2,:));
