﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Общие сведения об этой сборке предоставляются следующим набором
// атрибутов. Отредактируйте значения этих атрибутов, чтобы изменить
// общие сведения об этой сборке.
[assembly: AssemblyTitle("BarCodeScanner")]
[assembly: AssemblyDescription("Сканирование штрихкодов выпускаемой продукции на терминале сбора данных Casio IT-800R-15")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("JSC Greta, Druzhkivka, Ukraine")]
[assembly: AssemblyProduct("BarCodeScanner")]
[assembly: AssemblyCopyright("Copyright Oleg Sukhonos ©  2015-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Установка значения False в параметре ComVisible делает типы в этой сборке невидимыми
// для COM-компонентов. Если необходим доступ к типу в этой сборке из
// COM, следует установить атрибут ComVisible в TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("701cbabe-44ed-4108-969f-27ffc4a5fe9d")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии
//      Номер построения
//      Редакция
//
[assembly: AssemblyVersion("1.16.4.15")]

// Следующий атрибут служит для подавления предупреждения FxCop "CA2232: Microsoft.Usage: добавьте к сборке STAThreadAttribute",
// так как приложение для устройства не поддерживает поток STA.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2232:MarkWindowsFormsEntryPointsWithStaThread")]
