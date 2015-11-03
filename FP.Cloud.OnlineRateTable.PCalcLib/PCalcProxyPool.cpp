#include "PCalcProxyPool.hpp"
#include "PCalcProxy.hpp"

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::PCalcProxyPool()
{

}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::~PCalcProxyPool()
{
	this->!PCalcProxyPool();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::!PCalcProxyPool()
{
	if (nullptr != m_Pool)
	{
		delete m_Pool->Value;
		m_Pool = nullptr;
	}
}

Lib::PCalcProxy^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::Pop(Shared::EnvironmentInfo^ key)
{
	PCalcProxy^ result = nullptr;

	if (nullptr != m_Pool)
	{
		int keyValue = this->GetKeyValue(key);
		if (m_Pool->Key == keyValue)
		{
			result = m_Pool->Value;
		}

		//remove from stack
		m_Pool = nullptr;
	}

	return result;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::Push(Shared::EnvironmentInfo^ key, PCalcProxy^ proxy)
{
	if (nullptr != m_Pool)
	{
		PCalcProxy^ current = this->Pop(key);
		if (current != proxy)
		{
			delete current;
		}
	}

	m_Pool = gcnew System::Collections::Generic::KeyValuePair<int, PCalcProxy^>(GetKeyValue(key), proxy);
}

int FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyPool::GetKeyValue(Shared::EnvironmentInfo^ key)
{
	System::String^ zip = key->SenderZipCode == nullptr ? System::String::Empty : key->SenderZipCode;
	System::String^ culture = key->Culture == nullptr ? System::String::Empty : key->Culture;
	System::String^ countryCode = key->Iso3CountryCode == nullptr ? System::String::Empty : key->Iso3CountryCode;
	
	return key->CarrierId.GetHashCode() ^ key->UtcDate.GetHashCode() ^ zip->GetHashCode() ^culture->GetHashCode() ^ countryCode->GetHashCode();
}

