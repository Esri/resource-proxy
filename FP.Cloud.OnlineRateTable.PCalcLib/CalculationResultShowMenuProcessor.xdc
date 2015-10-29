<?xml version="1.0"?><doc>
<members>
<member name="M:_errno" decl="true" source="c:\projekte\fmlibs\boost\boost_1_59_0\boost\type_index.hpp" line="11">
\file boost/type_index.hpp
\brief Includes minimal set of headers required to use the Boost.TypeIndex library.

By inclusion of this file most optimal type index classes will be included and used 
as a boost::typeindex::type_index and boost::typeindex::type_info.
</member>
<member name="D:boost.typeindex.type_info" decl="false" source="c:\projekte\fmlibs\boost\boost_1_59_0\boost\type_index.hpp" line="137">
Depending on a compiler flags, optimal implementation of type_info will be used 
as a default boost::typeindex::type_info.

Could be a std::type_info, boost::typeindex::detail::ctti_data or 
some user defined class.

type_info \b is \b not copyable or default constructible. It is \b not assignable too!
</member>
</members>
</doc>