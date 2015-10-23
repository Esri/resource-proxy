
#include "Base/windows/OSFacade/WinMutex.hpp"

WinMutex::WinMutex()
	: m_Mutex(NULL),
	m_WasCreated(false)
{
}

WinMutex::~WinMutex()
{
}

bool WinMutex::Create(const char* pName)
{
	m_WasCreated = true;
	return m_WasCreated;
}

bool WinMutex::Create(const char* pName, const char* pPassword)
{
	m_WasCreated = true;
	return m_WasCreated;
}

void WinMutex::Delete()
{
	m_WasCreated = false;
}

bool WinMutex::Lock()
{
	return true;
}

bool WinMutex::Lock(const bpt::time_duration& rTimeout)
{
	return true;
}

bool WinMutex::Release()
{
	return true;
}
