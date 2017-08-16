#include "Base/misc/assert_m.h"
#include <string>
#include <vector>

#include "Base/misc/std_def.h"
#include "Base/misc/ConvertType.hpp"
#include "boost/cast.hpp"
#include "Convert.hpp"

using namespace System::Collections::Generic;
using namespace FP::Cloud::OnlineRateTable::PCalcLib;

namespace VariantSpecific
{
	public ref class MyZip2ZoneConverter abstract
	{
	public:
		static void SetInstance(MyZip2ZoneConverter^ value)
		{
			m_Instance = value;
		}

		static MyZip2ZoneConverter^ GetInstance()
		{
			return m_Instance;
		}

		virtual IEnumerable<INT32>^ GetZones(System::String^ currentZipCode, System::String^ targetZipCode) = 0;

	private:
		static MyZip2ZoneConverter^ m_Instance;
	};

	bool MapZip2Zone(const std::string& currentLocation, const std::string& targetLocation, std::vector<INT32>& zoneList)
	{
		System::String^ currentZipCode = Convert::ToString(currentLocation);
		System::String^ targetZipCode = Convert::ToString(targetLocation);

		MyZip2ZoneConverter^ converter = MyZip2ZoneConverter::GetInstance();
		if (nullptr == converter)
		{
			return false;
		}

		IEnumerable<INT32>^ result;
		try
		{
			result = converter->GetZones(currentZipCode, targetZipCode);
		}
		catch (System::Exception^)
		{
			return false;
		}

		zoneList.clear();
		for each(INT32 current in result)
		{
			zoneList.push_back(current);
		}
		return true;
	}
}