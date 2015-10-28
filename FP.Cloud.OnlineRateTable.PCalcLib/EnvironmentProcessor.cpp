#include "ProductCalculation/IPCalcConfiguration.hpp"
#include "EnvironmentProcessor.hpp"
#include "Convert.hpp"

USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::EnvironmentProcessor::EnvironmentProcessor(PCalcFactory^ factory)
	:m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::EnvironmentProcessor::Handle(EnvironmentInfo^ environment)
{
	if (nullptr == environment)
		return;

	IPCalcConfigurationPtr config = this->m_Factory->GetConfig();
	IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, Convert::ToString(environment->SenderZipCode));
	properties->SetValue(EnvironmentProperty::REQUEST_LANG, Convert::ToString(environment->Culture));
	properties->SetValue(EnvironmentProperty::REQUEST_GRAPHIC_FORMAT, 0);
	properties->SetValue(EnvironmentProperty::REQUEST_DYNAMIC_SCALE, 0);
	properties->SetValue(EnvironmentProperty::REQUEST_STATIC_SCALE, 1);
	properties->SetValue(EnvironmentProperty::REQUEST_MAXIMUM_WEIGHT, 2000);
	properties->SetValue(EnvironmentProperty::REQUEST_NON_ASCII_UI, 0);
	properties->SetValue(EnvironmentProperty::REQUEST_MEMORY_KEY_NO, 0);
	properties->SetValue(EnvironmentProperty::REQUEST_DISP_TEXT_LONG_LEN, 80);
	properties->SetValue(EnvironmentProperty::REQUEST_DISP_TEXT_SHORT_LEN, 40);
	properties->SetValue(EnvironmentProperty::REQUEST_METER_TYPE, 0);

	properties->SetValue(100, 0);
	properties->SetValue(101, 0);
	properties->SetValue(102, 0);
	properties->SetValue(103, 0);

	config->ChangeProperties(properties);

	this->m_Factory->GetPCalcMgr()->PCalcInitialize();
}

