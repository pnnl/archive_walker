/*
* MATLAB Compiler: 6.6 (R2018a)
* Date: Mon Mar 15 10:42:01 2021
* Arguments:
* "-B""macro_default""-W""dotnet:BAWSengine,GUI2MAT,4.0,private""-T""link:lib""-d""C:\GitF
* olders\archive_walker\MATengine\DLLs\BAWSengine_R2018a\for_testing""-v""class{GUI2MAT:C:
* \GitFolders\archive_walker\MATengine\GUIfunctions\GetFileExample.m,C:\GitFolders\archive
* _walker\MATengine\GUIfunctions\GetFileExampleDB.m,C:\GitFolders\archive_walker\MATengine
* \GUIfunctions\GetSparseData.m,C:\GitFolders\archive_walker\MATengine\GUIfunctions\Inspec
* tionAnalysis.m,C:\GitFolders\archive_walker\MATengine\GUIfunctions\ReadMMdata.m,C:\GitFo
* lders\archive_walker\MATengine\GUIfunctions\RerunForcedOscillation.m,C:\GitFolders\archi
* ve_walker\MATengine\GUIfunctions\RerunOutOfRange.m,C:\GitFolders\archive_walker\MATengin
* e\GUIfunctions\RerunRingdown.m,C:\GitFolders\archive_walker\MATengine\GUIfunctions\Retri
* eveData.m,C:\GitFolders\archive_walker\MATengine\GUIfunctions\RunNormalMode.m,C:\GitFold
* ers\archive_walker\MATengine\GUIfunctions\UpdateOBATpreset.m,C:\GitFolders\archive_walke
* r\MATengine\GUIfunctions\WriteMMreport.m}""-a""C:\GitFolders\archive_walker\MATengine\DQ
* andCustomization\SignalCustomization\GraphCustomization.m""-a""C:\GitFolders\archive_wal
* ker\MATengine\DQandCustomization\SignalCustomization\PCAcustomization.m""-a""C:\GitFolde
* rs\archive_walker\MATengine\DataProcessor\Filter\POWpmuFilt.m""-a""C:\GitFolders\archive
* _walker\MATengine\ReadingData\uPMUdatReader.m"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace BAWSengineNative
{

  /// <summary>
  /// The GUI2MAT class provides a CLS compliant, Object (native) interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\GetFileExample.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\GetFileExampleDB.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\GetSparseData.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\InspectionAnalysis.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\ReadMMdata.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\RerunForcedOscillation.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\RerunOutOfRange.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\RerunRingdown.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\RetrieveData.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\RunNormalMode.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\UpdateOBATpreset.m
  /// <newpara></newpara>
  /// C:\GitFolders\archive_walker\MATengine\GUIfunctions\WriteMMreport.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class GUI2MAT : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static GUI2MAT()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "BAWSengine.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the GUI2MAT class.
    /// </summary>
    public GUI2MAT()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~GUI2MAT()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExample()
    {
      return mcr.EvaluateFunction("GetFileExample", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExample(Object InputFile)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExample(Object InputFile, Object FileType)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile, FileType);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <param name="MetaOnly">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExample(Object InputFile, Object FileType, Object MetaOnly)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile, FileType, MetaOnly);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExample(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExample(int numArgsOut, Object InputFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExample(int numArgsOut, Object InputFile, Object FileType)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile, FileType);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <param name="MetaOnly">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExample(int numArgsOut, Object InputFile, Object FileType, 
                             Object MetaOnly)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile, FileType, MetaOnly);
    }


    /// <summary>
    /// Provides an interface for the GetFileExample function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("GetFileExample", 3, 1, 0)]
    protected void GetFileExample(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("GetFileExample", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB()
    {
      return mcr.EvaluateFunction("GetFileExampleDB", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB(Object StartTime)
    {
      return mcr.EvaluateFunction("GetFileExampleDB", StartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB(Object StartTime, Object preset)
    {
      return mcr.EvaluateFunction("GetFileExampleDB", StartTime, preset);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB(Object StartTime, Object preset, Object PresetFile)
    {
      return mcr.EvaluateFunction("GetFileExampleDB", StartTime, preset, PresetFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <param name="MetaOnly">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB(Object StartTime, Object preset, Object PresetFile, 
                             Object MetaOnly)
    {
      return mcr.EvaluateFunction("GetFileExampleDB", StartTime, preset, PresetFile, MetaOnly);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <param name="MetaOnly">Input argument #4</param>
    /// <param name="DBtype">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetFileExampleDB(Object StartTime, Object preset, Object PresetFile, 
                             Object MetaOnly, Object DBtype)
    {
      return mcr.EvaluateFunction("GetFileExampleDB", StartTime, preset, PresetFile, MetaOnly, DBtype);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut, Object StartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", StartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut, Object StartTime, Object preset)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", StartTime, preset);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut, Object StartTime, Object preset, 
                               Object PresetFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", StartTime, preset, PresetFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <param name="MetaOnly">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut, Object StartTime, Object preset, 
                               Object PresetFile, Object MetaOnly)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", StartTime, preset, PresetFile, MetaOnly);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the GetFileExampleDB MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="preset">Input argument #2</param>
    /// <param name="PresetFile">Input argument #3</param>
    /// <param name="MetaOnly">Input argument #4</param>
    /// <param name="DBtype">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetFileExampleDB(int numArgsOut, Object StartTime, Object preset, 
                               Object PresetFile, Object MetaOnly, Object DBtype)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExampleDB", StartTime, preset, PresetFile, MetaOnly, DBtype);
    }


    /// <summary>
    /// Provides an interface for the GetFileExampleDB function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Repeat error so that the message is captured by the GUI
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("GetFileExampleDB", 5, 1, 0)]
    protected void GetFileExampleDB(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("GetFileExampleDB", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetSparseData()
    {
      return mcr.EvaluateFunction("GetSparseData", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetSparseData(Object SparseStartTime)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetSparseData(Object SparseStartTime, Object SparseEndTime)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetSparseData(Object SparseStartTime, Object SparseEndTime, Object 
                          InitializationPath)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="SparseDetector">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object GetSparseData(Object SparseStartTime, Object SparseEndTime, Object 
                          InitializationPath, Object SparseDetector)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime, InitializationPath, SparseDetector);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetSparseData(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetSparseData(int numArgsOut, Object SparseStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetSparseData(int numArgsOut, Object SparseStartTime, Object 
                            SparseEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetSparseData(int numArgsOut, Object SparseStartTime, Object 
                            SparseEndTime, Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="SparseDetector">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] GetSparseData(int numArgsOut, Object SparseStartTime, Object 
                            SparseEndTime, Object InitializationPath, Object 
                            SparseDetector)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime, InitializationPath, SparseDetector);
    }


    /// <summary>
    /// Provides an interface for the GetSparseData function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("GetSparseData", 4, 1, 0)]
    protected void GetSparseData(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("GetSparseData", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the InspectionAnalysis
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object InspectionAnalysis()
    {
      return mcr.EvaluateFunction("InspectionAnalysis", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the InspectionAnalysis
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="Func">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object InspectionAnalysis(Object Func)
    {
      return mcr.EvaluateFunction("InspectionAnalysis", Func);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the InspectionAnalysis
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object InspectionAnalysis(Object Func, Object Data)
    {
      return mcr.EvaluateFunction("InspectionAnalysis", Func, Data);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the InspectionAnalysis
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <param name="t">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object InspectionAnalysis(Object Func, Object Data, Object t)
    {
      return mcr.EvaluateFunction("InspectionAnalysis", Func, Data, t);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the InspectionAnalysis
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <param name="t">Input argument #3</param>
    /// <param name="Params">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object InspectionAnalysis(Object Func, Object Data, Object t, Object Params)
    {
      return mcr.EvaluateFunction("InspectionAnalysis", Func, Data, t, Params);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the InspectionAnalysis MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] InspectionAnalysis(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "InspectionAnalysis", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the InspectionAnalysis MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="Func">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] InspectionAnalysis(int numArgsOut, Object Func)
    {
      return mcr.EvaluateFunction(numArgsOut, "InspectionAnalysis", Func);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the InspectionAnalysis MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] InspectionAnalysis(int numArgsOut, Object Func, Object Data)
    {
      return mcr.EvaluateFunction(numArgsOut, "InspectionAnalysis", Func, Data);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the InspectionAnalysis MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <param name="t">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] InspectionAnalysis(int numArgsOut, Object Func, Object Data, Object t)
    {
      return mcr.EvaluateFunction(numArgsOut, "InspectionAnalysis", Func, Data, t);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the InspectionAnalysis MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="Func">Input argument #1</param>
    /// <param name="Data">Input argument #2</param>
    /// <param name="t">Input argument #3</param>
    /// <param name="Params">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] InspectionAnalysis(int numArgsOut, Object Func, Object Data, Object 
                                 t, Object Params)
    {
      return mcr.EvaluateFunction(numArgsOut, "InspectionAnalysis", Func, Data, t, Params);
    }


    /// <summary>
    /// Provides an interface for the InspectionAnalysis function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("InspectionAnalysis", 4, 1, 0)]
    protected void InspectionAnalysis(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("InspectionAnalysis", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object ReadMMdata()
    {
      return mcr.EvaluateFunction("ReadMMdata", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object ReadMMdata(Object StartTime)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object ReadMMdata(Object StartTime, Object EndTime)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime, EndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object ReadMMdata(Object StartTime, Object EndTime, Object EventPath)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the ReadMMdata MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] ReadMMdata(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the ReadMMdata MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] ReadMMdata(int numArgsOut, Object StartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the ReadMMdata MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] ReadMMdata(int numArgsOut, Object StartTime, Object EndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime, EndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the ReadMMdata MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] ReadMMdata(int numArgsOut, Object StartTime, Object EndTime, Object 
                         EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides an interface for the ReadMMdata function in which the input and output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("ReadMMdata", 3, 1, 0)]
    protected void ReadMMdata(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("ReadMMdata", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation()
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime, 
                                   Object ConfigFile)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime, 
                                   Object ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime, 
                                   Object ConfigFile, Object ControlPath, Object 
                                   EventPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime, 
                                   Object ConfigFile, Object ControlPath, Object 
                                   EventPath, Object InitializationPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input Objectinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunForcedOscillation(Object RerunStartTime, Object RerunEndTime, 
                                   Object ConfigFile, Object ControlPath, Object 
                                   EventPath, Object InitializationPath, Object 
                                   FileDirectory)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime, Object ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime, Object ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime, Object ConfigFile, Object ControlPath, 
                                     Object EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime, Object ConfigFile, Object ControlPath, 
                                     Object EventPath, Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input Object interface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunForcedOscillation(int numArgsOut, Object RerunStartTime, Object 
                                     RerunEndTime, Object ConfigFile, Object ControlPath, 
                                     Object EventPath, Object InitializationPath, Object 
                                     FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunForcedOscillation function in which the input
    /// and output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("RerunForcedOscillation", 7, 1, 0)]
    protected void RerunForcedOscillation(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("RerunForcedOscillation", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange()
    {
      return mcr.EvaluateFunction("RerunOutOfRange", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime, Object 
                            ConfigFile)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime, Object 
                            ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime, Object 
                            ConfigFile, Object ControlPath, Object EventPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime, Object 
                            ConfigFile, Object ControlPath, Object EventPath, Object 
                            InitializationPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input Objectinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunOutOfRange(Object RerunStartTime, Object RerunEndTime, Object 
                            ConfigFile, Object ControlPath, Object EventPath, Object 
                            InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime, Object ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime, Object ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                              EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                              EventPath, Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input Object interface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunOutOfRange(int numArgsOut, Object RerunStartTime, Object 
                              RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                              EventPath, Object InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunOutOfRange function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("RerunOutOfRange", 7, 1, 0)]
    protected void RerunOutOfRange(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("RerunOutOfRange", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown()
    {
      return mcr.EvaluateFunction("RerunRingdown", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime, Object 
                          ConfigFile)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime, Object 
                          ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime, Object 
                          ConfigFile, Object ControlPath, Object EventPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime, Object 
                          ConfigFile, Object ControlPath, Object EventPath, Object 
                          InitializationPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input Objectinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RerunRingdown(Object RerunStartTime, Object RerunEndTime, Object 
                          ConfigFile, Object ControlPath, Object EventPath, Object 
                          InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime, Object ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime, Object ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                            EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                            EventPath, Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input Object interface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RerunRingdown(int numArgsOut, Object RerunStartTime, Object 
                            RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                            EventPath, Object InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunRingdown function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("RerunRingdown", 7, 1, 0)]
    protected void RerunRingdown(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("RerunRingdown", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData()
    {
      return mcr.EvaluateFunction("RetrieveData", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime, Object 
                         ConfigFile)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime, Object 
                         ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime, Object 
                         ConfigFile, Object ControlPath, Object EventPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime, Object 
                         ConfigFile, Object ControlPath, Object EventPath, Object 
                         InitializationPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input Objectinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object RetrieveData(Object RerunStartTime, Object RerunEndTime, Object 
                         ConfigFile, Object ControlPath, Object EventPath, Object 
                         InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime, Object ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime, Object ConfigFile, Object ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                           EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                           EventPath, Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input Object interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <param name="EventPath">Input argument #5</param>
    /// <param name="InitializationPath">Input argument #6</param>
    /// <param name="FileDirectory">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RetrieveData(int numArgsOut, Object RerunStartTime, Object 
                           RerunEndTime, Object ConfigFile, Object ControlPath, Object 
                           EventPath, Object InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RetrieveData function in which the input and output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("RetrieveData", 7, 1, 0)]
    protected void RetrieveData(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("RetrieveData", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a void output, 0-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    ///
    public void RunNormalMode()
    {
      mcr.EvaluateFunction(0, "RunNormalMode", new Object[]{});
    }


    /// <summary>
    /// Provides a void output, 1-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    ///
    public void RunNormalMode(Object ControlPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath);
    }


    /// <summary>
    /// Provides a void output, 2-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    ///
    public void RunNormalMode(Object ControlPath, Object EventPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a void output, 3-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    ///
    public void RunNormalMode(Object ControlPath, Object EventPath, Object 
                        InitializationPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a void output, 4-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="FileDirectory">Input argument #4</param>
    ///
    public void RunNormalMode(Object ControlPath, Object EventPath, Object 
                        InitializationPath, Object FileDirectory)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides a void output, 5-input Objectinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="FileDirectory">Input argument #4</param>
    /// <param name="ConfigFile">Input argument #5</param>
    ///
    public void RunNormalMode(Object ControlPath, Object EventPath, Object 
                        InitializationPath, Object FileDirectory, Object ConfigFile)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut, Object ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut, Object ControlPath, Object EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut, Object ControlPath, Object EventPath, 
                            Object InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="FileDirectory">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut, Object ControlPath, Object EventPath, 
                            Object InitializationPath, Object FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="FileDirectory">Input argument #4</param>
    /// <param name="ConfigFile">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] RunNormalMode(int numArgsOut, Object ControlPath, Object EventPath, 
                            Object InitializationPath, Object FileDirectory, Object 
                            ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory, ConfigFile);
    }


    /// <summary>
    /// Provides an interface for the RunNormalMode function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("RunNormalMode", 5, 0, 0)]
    protected void RunNormalMode(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("RunNormalMode", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object UpdateOBATpreset()
    {
      return mcr.EvaluateFunction("UpdateOBATpreset", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="NewPreset">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object UpdateOBATpreset(Object NewPreset)
    {
      return mcr.EvaluateFunction("UpdateOBATpreset", NewPreset);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object UpdateOBATpreset(Object NewPreset, Object Detector)
    {
      return mcr.EvaluateFunction("UpdateOBATpreset", NewPreset, Detector);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <param name="AWconfigFile">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object UpdateOBATpreset(Object NewPreset, Object Detector, Object AWconfigFile)
    {
      return mcr.EvaluateFunction("UpdateOBATpreset", NewPreset, Detector, AWconfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <param name="AWconfigFile">Input argument #3</param>
    /// <param name="OBATpresetFile">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object UpdateOBATpreset(Object NewPreset, Object Detector, Object 
                             AWconfigFile, Object OBATpresetFile)
    {
      return mcr.EvaluateFunction("UpdateOBATpreset", NewPreset, Detector, AWconfigFile, OBATpresetFile);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] UpdateOBATpreset(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "UpdateOBATpreset", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="NewPreset">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] UpdateOBATpreset(int numArgsOut, Object NewPreset)
    {
      return mcr.EvaluateFunction(numArgsOut, "UpdateOBATpreset", NewPreset);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] UpdateOBATpreset(int numArgsOut, Object NewPreset, Object Detector)
    {
      return mcr.EvaluateFunction(numArgsOut, "UpdateOBATpreset", NewPreset, Detector);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <param name="AWconfigFile">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] UpdateOBATpreset(int numArgsOut, Object NewPreset, Object Detector, 
                               Object AWconfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "UpdateOBATpreset", NewPreset, Detector, AWconfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the UpdateOBATpreset MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="NewPreset">Input argument #1</param>
    /// <param name="Detector">Input argument #2</param>
    /// <param name="AWconfigFile">Input argument #3</param>
    /// <param name="OBATpresetFile">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] UpdateOBATpreset(int numArgsOut, Object NewPreset, Object Detector, 
                               Object AWconfigFile, Object OBATpresetFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "UpdateOBATpreset", NewPreset, Detector, AWconfigFile, OBATpresetFile);
    }


    /// <summary>
    /// Provides an interface for the UpdateOBATpreset function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Read the OBAT preset file into cell A
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("UpdateOBATpreset", 4, 1, 0)]
    protected void UpdateOBATpreset(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("UpdateOBATpreset", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport()
    {
      return mcr.EvaluateFunction("WriteMMreport", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime, Object EventPath)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides a single output, 4-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime, Object EventPath, 
                          Object ReportType)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime, EventPath, ReportType);
    }


    /// <summary>
    /// Provides a single output, 5-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime, Object EventPath, 
                          Object ReportType, Object DampThresh)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh);
    }


    /// <summary>
    /// Provides a single output, 6-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <param name="EventSepMinutes">Input argument #6</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime, Object EventPath, 
                          Object ReportType, Object DampThresh, Object EventSepMinutes)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh, EventSepMinutes);
    }


    /// <summary>
    /// Provides a single output, 7-input Objectinterface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <param name="EventSepMinutes">Input argument #6</param>
    /// <param name="ReportPath">Input argument #7</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object WriteMMreport(Object StartTime, Object EndTime, Object EventPath, 
                          Object ReportType, Object DampThresh, Object EventSepMinutes, 
                          Object ReportPath)
    {
      return mcr.EvaluateFunction("WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh, EventSepMinutes, ReportPath);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime);
    }


    /// <summary>
    /// Provides the standard 3-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime, 
                            Object EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides the standard 4-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime, 
                            Object EventPath, Object ReportType)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime, EventPath, ReportType);
    }


    /// <summary>
    /// Provides the standard 5-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime, 
                            Object EventPath, Object ReportType, Object DampThresh)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh);
    }


    /// <summary>
    /// Provides the standard 6-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <param name="EventSepMinutes">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime, 
                            Object EventPath, Object ReportType, Object DampThresh, 
                            Object EventSepMinutes)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh, EventSepMinutes);
    }


    /// <summary>
    /// Provides the standard 7-input Object interface to the WriteMMreport MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <param name="ReportType">Input argument #4</param>
    /// <param name="DampThresh">Input argument #5</param>
    /// <param name="EventSepMinutes">Input argument #6</param>
    /// <param name="ReportPath">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] WriteMMreport(int numArgsOut, Object StartTime, Object EndTime, 
                            Object EventPath, Object ReportType, Object DampThresh, 
                            Object EventSepMinutes, Object ReportPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "WriteMMreport", StartTime, EndTime, EventPath, ReportType, DampThresh, EventSepMinutes, ReportPath);
    }


    /// <summary>
    /// Provides an interface for the WriteMMreport function in which the input and
    /// output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Identify periods separated by at least EventSepSamp samples
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("WriteMMreport", 7, 1, 0)]
    protected void WriteMMreport(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("WriteMMreport", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }

    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
