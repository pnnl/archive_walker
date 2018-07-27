%   function Threshold = CalcThreshold(GMSC_est, Pfa, LengthFreqInterest,NSigs)
%   This function calculates threshold for detecting forced oscillations
%   Inputs:
%           GMSC_est: Estimate of generalized maagnitude squared coherence
%           Pfa: Probability of false alarmfrequency bind to be considered
%           Window: a vector containing data point for windowing signal
%           MedFiltOrd: Order of median filter. If empty, median filter is
%           not applied to the PSD
%           NSigs: Number of Signals
%
%   Outputs:
%           Threshold: Threshold for detecting Forced Oscillations
%           f: frequency of estimated power spectrum
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016

function Threshold = CalcThreshold(GMSC_est, Pfa, LengthFreqInterest,NSigs)
%calculates threshold assuming all signals are independent
Gamma_Ind =  chi2inv(1-Pfa/LengthFreqInterest, 2*NSigs);
%calculates threshold assuming all signals are identical
Gamma_Iden = NSigs*chi2inv(1-Pfa/LengthFreqInterest, 2);
%recalculates threshold using GMSC estimate, Gamma_Ind and Gamma_Iden
Threshold = Gamma_Ind*(1-GMSC_est) + Gamma_Iden*GMSC_est;

end