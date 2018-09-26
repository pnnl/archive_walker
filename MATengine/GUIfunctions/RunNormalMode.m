% function RunNormalMode(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile)
%
% This is one of the top-level functions intended to be called by the GUI.
% It is a wrapper for BAWS_main9 used to perform the initial analysis of
% data.
%
% Called by: 
%   The AW GUI
%
% Calls: BAWS_main9
%
% Inputs:
%   ControlPath - Path to folders containing Run.txt and Pause.txt files
%       written by the GUI to control the AW engine. A string.
%   EventPath - Path to the folder where results from detectors are to be
%       stored. A string.
%   InitializationPath - Path to the folder where initialization files
%       (used in rerun mode to recreate detailed results) and sparse data
%       (max and min of analyzed signals) are stored. A string.
%   FileDirectory - Paths to where PMU data that is to be analyzed is
%       stored. Cell array of strings.
%   ConfigFile - Path to the configuration XML used to configure the AW
%       engine for a run.
%
% Outputs: none

function RunNormalMode(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile)

BAWS_main9(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile);