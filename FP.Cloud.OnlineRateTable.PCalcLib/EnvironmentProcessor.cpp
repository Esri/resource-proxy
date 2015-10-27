#include "ProductCalculation/IPCalcConfiguration.hpp"
#include "EnvironmentProcessor.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
BEGIN_PCALC_LIB_NAMESPACE

EnvironmentProcessor::EnvironmentProcessor(PCalcFactory^ factory)
	:m_Factory(factory)
{

}

END_PCALC_LIB_NAMESPACE

void FP::Cloud::OnlineRateTable::PCalcLib::EnvironmentProcessor::Handle(EnvironmentInfo^ environment)
{
	if (nullptr == environment)
		return;

	IPCalcConfigurationPtr config = this->m_Factory->GetConfig();
	IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, PCalcManagedLib::ConvertToString(environment->SenderZipCode));
	properties->SetValue(EnvironmentProperty::REQUEST_LANG, PCalcManagedLib::ConvertToString(environment->Culture));
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

