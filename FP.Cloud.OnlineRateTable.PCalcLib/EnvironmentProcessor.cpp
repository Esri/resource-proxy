#include "ProductCalculation/IPCalcConfiguration.hpp"
#include "EnvironmentProcessor.hpp"
#include "Convert.hpp"
#include "PCalcFactory.hpp"

FP::Cloud::OnlineRateTable::PCalcLib::EnvironmentProcessor::EnvironmentProcessor(PCalcFactory^ factory)
	:m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::EnvironmentProcessor::Handle(Shared::EnvironmentInfo^ environment)
{
	if (nullptr == environment)
		return;

	PT::IPCalcConfigurationPtr config = this->m_Factory->GetConfig();
	PT::IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(PT::EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, Convert::ToString(environment->SenderZipCode));
	properties->SetValue(PT::EnvironmentProperty::REQUEST_LANG, Convert::ToString(environment->Culture));
	properties->SetValue(PT::EnvironmentProperty::REQUEST_GRAPHIC_FORMAT, 0);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_DYNAMIC_SCALE, 0);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_STATIC_SCALE, 1);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_MAXIMUM_WEIGHT, 2000);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_NON_ASCII_UI, 0);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_MEMORY_KEY_NO, 0);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_DISP_TEXT_LONG_LEN, 80);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_DISP_TEXT_SHORT_LEN, 40);
	properties->SetValue(PT::EnvironmentProperty::REQUEST_METER_TYPE, 0);

	properties->SetValue(100, 0);
	properties->SetValue(101, 0);
	properties->SetValue(102, 0);
	properties->SetValue(103, 0);

	config->ChangeProperties(properties);

	this->m_Factory->GetPCalcMgr()->PCalcInitialize();
}

