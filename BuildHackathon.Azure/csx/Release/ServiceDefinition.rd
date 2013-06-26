<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="BuildHackathon.Azure" generation="1" functional="0" release="0" Id="03ccde03-119d-4e38-82af-7262cd30457d" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="BuildHackathon.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="BuildHackathon:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/LB:BuildHackathon:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="BuildHackathon:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/MapBuildHackathon:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="BuildHackathonInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/MapBuildHackathonInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:BuildHackathon:Endpoint1">
          <toPorts>
            <inPortMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathon/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapBuildHackathon:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathon/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapBuildHackathonInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathonInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="BuildHackathon" generation="1" functional="0" release="0" software="D:\git\BuildHackathon\BuildHackathon.Azure\csx\Release\roles\BuildHackathon" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;BuildHackathon&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BuildHackathon&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathonInstances" />
            <sCSPolicyUpdateDomainMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathonUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathonFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="BuildHackathonUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="BuildHackathonFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="BuildHackathonInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="e3b05f18-7ade-4c2a-8672-27145d294dc3" ref="Microsoft.RedDog.Contract\ServiceContract\BuildHackathon.AzureContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="2c2692f1-b0a5-4902-a95e-40f6d69b4fb3" ref="Microsoft.RedDog.Contract\Interface\BuildHackathon:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/BuildHackathon.Azure/BuildHackathon.AzureGroup/BuildHackathon:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>