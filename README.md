# DHBW_FN_AI
Having code for cognitive service and custom vision

**This is a .Net Core 2.1 console application**
It takes an image as input and sends it to the Azure Custom Vision Prediction API, to retrieve results as json

The result could be parsed for example with PowerShell   

`
PS C:\DHBW_FN_AI> $json = Predict-console.exe | ConvertFrom-Json
PS C:\DHBW_FN_AI> $json
`
```
id          : 7927754e-6a8e-4aa9-b51f-059f130ac3af
project     : 0ea8a39b-3c69-49ca-8fb6-66c91df28cd0
iteration   : 778c267f-1303-444a-aa20-8dffd0197785
created     : 2018-12-02T17:01:56.5194304Z
predictions : {@{probability=1,0; tagId=dafa5497-d43f-409c-8446-5865f7afb745; tagName=Tasse}, @{probability=4,13292761E-10; tagId=d062f9d3-d821-4d26-957e-34598355743a;
              tagName=Glühbirne}}```

PS C:\DHBW_FN_AI> $json.predictions

   probability tagId                                tagName
   ----------- -----                                -------
           1,0 dafa5497-d43f-409c-8446-5865f7afb745 Tasse
4,13292761E-10 d062f9d3-d821-4d26-957e-34598355743a Glühbirne
`