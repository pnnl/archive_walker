/*
* MATLAB Compiler: 6.4 (R2017a)
* Date: Tue Dec 11 08:12:38 2018
* Arguments:
* "-B""macro_default""-W""dotnet:BAWSengine,GUI2MAT,4.0,private""-T""link:lib""-d""C:\User
* s\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\DLLs\BAWSengine_2_7\for_tes
* ting""-v""class{GUI2MAT:C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATeng
* ine\GUIfunctions\GetFileExample.m,C:\Users\foll154\Documents\BPAoscillationApp\AWreposit
* ory\MATengine\GUIfunctions\GetSparseData.m,C:\Users\foll154\Documents\BPAoscillationApp\
* AWrepository\MATengine\GUIfunctions\ReadMMdata.m,C:\Users\foll154\Documents\BPAoscillati
* onApp\AWrepository\MATengine\GUIfunctions\RerunForcedOscillation.m,C:\Users\foll154\Docu
* ments\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\RerunOutOfRange.m,C:\Users\f
* oll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\RerunRingdown.m,C
* :\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\RerunThe
* venin.m,C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions
* \RetrieveData.m,C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIf
* unctions\RunNormalMode.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace BAWSengine
{

  /// <summary>
  /// The GUI2MAT class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Get
  /// FileExample.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Get
  /// SparseData.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Rea
  /// dMMdata.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Rer
  /// unForcedOscillation.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Rer
  /// unOutOfRange.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Rer
  /// unRingdown.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Rer
  /// unThevenin.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Ret
  /// rieveData.m
  /// <newpara></newpara>
  /// C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\GUIfunctions\Run
  /// NormalMode.m
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
    /// Provides a single output, 0-input MWArrayinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetFileExample()
    {
      return mcr.EvaluateFunction("GetFileExample", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetFileExample(MWArray InputFile)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetFileExample(MWArray InputFile, MWArray FileType)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile, FileType);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <param name="MetaOnly">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetFileExample(MWArray InputFile, MWArray FileType, MWArray MetaOnly)
    {
      return mcr.EvaluateFunction("GetFileExample", InputFile, FileType, MetaOnly);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] GetFileExample(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] GetFileExample(int numArgsOut, MWArray InputFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] GetFileExample(int numArgsOut, MWArray InputFile, MWArray FileType)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile, FileType);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the GetFileExample MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="InputFile">Input argument #1</param>
    /// <param name="FileType">Input argument #2</param>
    /// <param name="MetaOnly">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] GetFileExample(int numArgsOut, MWArray InputFile, MWArray FileType, 
                              MWArray MetaOnly)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetFileExample", InputFile, FileType, MetaOnly);
    }


    /// <summary>
    /// Provides an interface for the GetFileExample function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void GetFileExample(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("GetFileExample", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetSparseData()
    {
      return mcr.EvaluateFunction("GetSparseData", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetSparseData(MWArray SparseStartTime)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetSparseData(MWArray SparseStartTime, MWArray SparseEndTime)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the GetSparseData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="SparseStartTime">Input argument #1</param>
    /// <param name="SparseEndTime">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetSparseData(MWArray SparseStartTime, MWArray SparseEndTime, MWArray 
                           InitializationPath)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the GetSparseData MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray GetSparseData(MWArray SparseStartTime, MWArray SparseEndTime, MWArray 
                           InitializationPath, MWArray SparseDetector)
    {
      return mcr.EvaluateFunction("GetSparseData", SparseStartTime, SparseEndTime, InitializationPath, SparseDetector);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the GetSparseData MATLAB
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
    public MWArray[] GetSparseData(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the GetSparseData MATLAB
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
    public MWArray[] GetSparseData(int numArgsOut, MWArray SparseStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the GetSparseData MATLAB
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
    public MWArray[] GetSparseData(int numArgsOut, MWArray SparseStartTime, MWArray 
                             SparseEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the GetSparseData MATLAB
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
    public MWArray[] GetSparseData(int numArgsOut, MWArray SparseStartTime, MWArray 
                             SparseEndTime, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the GetSparseData MATLAB
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
    public MWArray[] GetSparseData(int numArgsOut, MWArray SparseStartTime, MWArray 
                             SparseEndTime, MWArray InitializationPath, MWArray 
                             SparseDetector)
    {
      return mcr.EvaluateFunction(numArgsOut, "GetSparseData", SparseStartTime, SparseEndTime, InitializationPath, SparseDetector);
    }


    /// <summary>
    /// Provides an interface for the GetSparseData function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Convert string times to datetime
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void GetSparseData(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("GetSparseData", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ReadMMdata()
    {
      return mcr.EvaluateFunction("ReadMMdata", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ReadMMdata(MWArray StartTime)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ReadMMdata(MWArray StartTime, MWArray EndTime)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime, EndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <param name="EventPath">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ReadMMdata(MWArray StartTime, MWArray EndTime, MWArray EventPath)
    {
      return mcr.EvaluateFunction("ReadMMdata", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ReadMMdata(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ReadMMdata(int numArgsOut, MWArray StartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the ReadMMdata MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="StartTime">Input argument #1</param>
    /// <param name="EndTime">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ReadMMdata(int numArgsOut, MWArray StartTime, MWArray EndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime, EndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the ReadMMdata MATLAB
    /// function.
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
    public MWArray[] ReadMMdata(int numArgsOut, MWArray StartTime, MWArray EndTime, 
                          MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "ReadMMdata", StartTime, EndTime, EventPath);
    }


    /// <summary>
    /// Provides an interface for the ReadMMdata function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void ReadMMdata(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("ReadMMdata", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation()
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the RerunForcedOscillation
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime, 
                                    MWArray ConfigFile)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the RerunForcedOscillation
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime, 
                                    MWArray ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the RerunForcedOscillation
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime, 
                                    MWArray ConfigFile, MWArray ControlPath, MWArray 
                                    EventPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the RerunForcedOscillation
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime, 
                                    MWArray ConfigFile, MWArray ControlPath, MWArray 
                                    EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the RerunForcedOscillation
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunForcedOscillation(MWArray RerunStartTime, MWArray RerunEndTime, 
                                    MWArray ConfigFile, MWArray ControlPath, MWArray 
                                    EventPath, MWArray InitializationPath, MWArray 
                                    FileDirectory)
    {
      return mcr.EvaluateFunction("RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime, MWArray ConfigFile, MWArray 
                                      ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime, MWArray ConfigFile, MWArray 
                                      ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime, MWArray ConfigFile, MWArray 
                                      ControlPath, MWArray EventPath, MWArray 
                                      InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the RerunForcedOscillation
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
    public MWArray[] RerunForcedOscillation(int numArgsOut, MWArray RerunStartTime, 
                                      MWArray RerunEndTime, MWArray ConfigFile, MWArray 
                                      ControlPath, MWArray EventPath, MWArray 
                                      InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunForcedOscillation", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunForcedOscillation function in which the input
    /// and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void RerunForcedOscillation(int numArgsOut, ref MWArray[] argsOut, MWArray[] 
                             argsIn)
    {
      mcr.EvaluateFunction("RerunForcedOscillation", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange()
    {
      return mcr.EvaluateFunction("RerunOutOfRange", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the RerunOutOfRange MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                             ConfigFile)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the RerunOutOfRange MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                             ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the RerunOutOfRange MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                             ConfigFile, MWArray ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the RerunOutOfRange MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                             ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                             InitializationPath)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the RerunOutOfRange MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunOutOfRange(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                             ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                             InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction("RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime, MWArray ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                               MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                               MWArray EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the RerunOutOfRange MATLAB
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
    public MWArray[] RerunOutOfRange(int numArgsOut, MWArray RerunStartTime, MWArray 
                               RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                               MWArray EventPath, MWArray InitializationPath, MWArray 
                               FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunOutOfRange", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunOutOfRange function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void RerunOutOfRange(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("RerunOutOfRange", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown()
    {
      return mcr.EvaluateFunction("RerunRingdown", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the RerunRingdown MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the RerunRingdown MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the RerunRingdown MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the RerunRingdown MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                           InitializationPath)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the RerunRingdown MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunRingdown(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                           InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction("RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the RerunRingdown MATLAB
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
    public MWArray[] RerunRingdown(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath, MWArray InitializationPath, MWArray 
                             FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunRingdown", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RerunRingdown function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void RerunRingdown(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("RerunRingdown", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the RerunThevenin MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin()
    {
      return mcr.EvaluateFunction("RerunThevenin", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the RerunThevenin MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the RerunThevenin MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the RerunThevenin MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the RerunThevenin MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the RerunThevenin MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the RerunThevenin MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                           InitializationPath)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the RerunThevenin MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                           InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides a single output, 8-input MWArrayinterface to the RerunThevenin MATLAB
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
    /// <param name="PredictionDelay">Input argument #8</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RerunThevenin(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                           ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                           InitializationPath, MWArray FileDirectory, MWArray 
                           PredictionDelay)
    {
      return mcr.EvaluateFunction("RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory, PredictionDelay);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the RerunThevenin MATLAB
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
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath, MWArray InitializationPath, MWArray 
                             FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 8-input MWArray interface to the RerunThevenin MATLAB
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
    /// <param name="PredictionDelay">Input argument #8</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] RerunThevenin(int numArgsOut, MWArray RerunStartTime, MWArray 
                             RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                             MWArray EventPath, MWArray InitializationPath, MWArray 
                             FileDirectory, MWArray PredictionDelay)
    {
      return mcr.EvaluateFunction(numArgsOut, "RerunThevenin", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory, PredictionDelay);
    }


    /// <summary>
    /// Provides an interface for the RerunThevenin function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// Rerun the general out-of-range detector for the specified time period
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void RerunThevenin(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("RerunThevenin", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData()
    {
      return mcr.EvaluateFunction("RetrieveData", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                          ConfigFile)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="RerunStartTime">Input argument #1</param>
    /// <param name="RerunEndTime">Input argument #2</param>
    /// <param name="ConfigFile">Input argument #3</param>
    /// <param name="ControlPath">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                          ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the RetrieveData MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                          ConfigFile, MWArray ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the RetrieveData MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                          ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                          InitializationPath)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the RetrieveData MATLAB
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
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray RetrieveData(MWArray RerunStartTime, MWArray RerunEndTime, MWArray 
                          ConfigFile, MWArray ControlPath, MWArray EventPath, MWArray 
                          InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction("RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RetrieveData MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] RetrieveData(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime, MWArray ConfigFile, MWArray ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                            MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                            MWArray EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the RetrieveData MATLAB
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
    public MWArray[] RetrieveData(int numArgsOut, MWArray RerunStartTime, MWArray 
                            RerunEndTime, MWArray ConfigFile, MWArray ControlPath, 
                            MWArray EventPath, MWArray InitializationPath, MWArray 
                            FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RetrieveData", RerunStartTime, RerunEndTime, ConfigFile, ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides an interface for the RetrieveData function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void RetrieveData(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("RetrieveData", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a void output, 0-input MWArrayinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    ///
    public void RunNormalMode()
    {
      mcr.EvaluateFunction(0, "RunNormalMode", new MWArray[]{});
    }


    /// <summary>
    /// Provides a void output, 1-input MWArrayinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    ///
    public void RunNormalMode(MWArray ControlPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath);
    }


    /// <summary>
    /// Provides a void output, 2-input MWArrayinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    ///
    public void RunNormalMode(MWArray ControlPath, MWArray EventPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath);
    }


    /// <summary>
    /// Provides a void output, 3-input MWArrayinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    ///
    public void RunNormalMode(MWArray ControlPath, MWArray EventPath, MWArray 
                        InitializationPath)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides a void output, 4-input MWArrayinterface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="ControlPath">Input argument #1</param>
    /// <param name="EventPath">Input argument #2</param>
    /// <param name="InitializationPath">Input argument #3</param>
    /// <param name="FileDirectory">Input argument #4</param>
    ///
    public void RunNormalMode(MWArray ControlPath, MWArray EventPath, MWArray 
                        InitializationPath, MWArray FileDirectory)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides a void output, 5-input MWArrayinterface to the RunNormalMode MATLAB
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
    public void RunNormalMode(MWArray ControlPath, MWArray EventPath, MWArray 
                        InitializationPath, MWArray FileDirectory, MWArray ConfigFile)
    {
      mcr.EvaluateFunction(0, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory, ConfigFile);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] RunNormalMode(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the RunNormalMode MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="ControlPath">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] RunNormalMode(int numArgsOut, MWArray ControlPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the RunNormalMode MATLAB
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
    public MWArray[] RunNormalMode(int numArgsOut, MWArray ControlPath, MWArray EventPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the RunNormalMode MATLAB
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
    public MWArray[] RunNormalMode(int numArgsOut, MWArray ControlPath, MWArray 
                             EventPath, MWArray InitializationPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the RunNormalMode MATLAB
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
    public MWArray[] RunNormalMode(int numArgsOut, MWArray ControlPath, MWArray 
                             EventPath, MWArray InitializationPath, MWArray FileDirectory)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the RunNormalMode MATLAB
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
    public MWArray[] RunNormalMode(int numArgsOut, MWArray ControlPath, MWArray 
                             EventPath, MWArray InitializationPath, MWArray 
                             FileDirectory, MWArray ConfigFile)
    {
      return mcr.EvaluateFunction(numArgsOut, "RunNormalMode", ControlPath, EventPath, InitializationPath, FileDirectory, ConfigFile);
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
