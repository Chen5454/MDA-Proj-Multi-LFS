/*
Copyright (c) 2014-2018 by Mercer Road Corp

Permission to use, copy, modify or distribute this software in binary or source form
for any purpose is allowed only under explicit prior consent in writing from Mercer Road Corp

THE SOFTWARE IS PROVIDED "AS IS" AND MERCER ROAD CORP DISCLAIMS
ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL MERCER ROAD CORP
BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL
DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR
PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS
ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS
SOFTWARE.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using VivoxUnity.Common;

namespace VivoxUnity.Private
{

    internal class AudioInputDevices : IAudioDevices
    {

        #region Member Variables
        private readonly string DefaultSystemDevice = "Default System Device";
        private readonly string DefaultCommunicationDevice = "Default Communication Device";
        private AudioDevice _defaultSystemDevice;
        private AudioDevice _defaultCommunicationDevice;
        private AudioDevice _currentSystemDevice;
        private AudioDevice _currentCommunicationDevice;
        private AudioDevice _activeDevice;
        private AudioDevice _effectiveDevice;
        private int _audioGain;
        private bool _muted;

        private readonly VxClient _client;
        private readonly ReadWriteDictionary<string, IAudioDevice, AudioDevice> _devices = new ReadWriteDictionary<string, IAudioDevice, AudioDevice>();

        #endregion

        #region Helpers

        int ConvertGain(int gain)
        {
            return gain + 50;
        }

        #endregion

        public AudioInputDevices(VxClient client)
        {
            _client = client;
            _defaultSystemDevice = new AudioDevice { Key = DefaultSystemDevice, Name = DefaultSystemDevice };
            _defaultCommunicationDevice = new AudioDevice { Key = DefaultCommunicationDevice, Name = DefaultCommunicationDevice };
            _currentSystemDevice = new AudioDevice { Key = DefaultSystemDevice, Name = DefaultSystemDevice };
            _currentCommunicationDevice = new AudioDevice { Key = DefaultCommunicationDevice, Name = DefaultCommunicationDevice };
            _activeDevice = _defaultSystemDevice;

            VxClient.Instance.EventMessageReceived += OnEventMessageReceived;
        }

        #region IAudioDevices

        public event PropertyChangedEventHandler PropertyChanged;

        public IAudioDevice SystemDevice => _defaultSystemDevice;
        public IAudioDevice CommunicationDevice => _defaultCommunicationDevice;
        public IAudioDevice ActiveDevice => _activeDevice;
        public IAudioDevice EffectiveDevice => _effectiveDevice;
        public IReadOnlyDictionary<string, IAudioDevice> AvailableDevices => _devices;

        public IAsyncResult BeginSetActiveDevice(IAudioDevice device, AsyncCallback callback)
        {
            if (device == null) throw new ArgumentNullException();

            AsyncNoResult result = new AsyncNoResult(callback);
            var request = new vx_req_aux_set_capture_device_t();
            request.capture_device_specifier = device.Key;
            return _client.BeginIssueRequest(request, ar =>
            {
                try
                {
                    _client.EndIssueRequest(ar);

                    // When trying to set the active device to what is already the active device, return.
                    if (_activeDevice.Key == device.Key)
                    {
                        return;
                    }
                    _activeDevice = (AudioDevice)device;

                    if (_activeDevice == AvailableDevices[DefaultSystemDevice])
                    {
                        _effectiveDevice = new AudioDevice
                        {
                            Key = _currentSystemDevice.Key,
                            Name = _currentSystemDevice.Name
                        };
                    }
                    else if (_activeDevice == AvailableDevices[DefaultCommunicationDevice])
                    {
                        _effectiveDevice = new AudioDevice
                        {
                            Key = _currentCommunicationDevice.Key,
                            Name = _currentCommunicationDevice.Name
                        };
                    }
                    else
                    {
                        _effectiveDevice = new AudioDevice
                        {
                            Key = device.Key,
                            Name = device.Name
                        };
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EffectiveDevice)));

                    result.SetComplete();
                }
                catch (Exception e)
                {
                    VivoxDebug.Instance.VxExceptionMessage($"{request.GetType().Name} failed: {e}");
                    result.SetComplete(e);
                    if (VivoxDebug.Instance.throwInternalExcepetions)
                    {
                        throw;
                    }
                }
            });
        }

        public void EndSetActiveDevice(IAsyncResult result)
        {
            ((AsyncNoResult)result).CheckForError();
        }

        public int VolumeAdjustment
        {
            get { return _audioGain; }
            set
            {
                if (value < -50 || value > 50)
                    throw new ArgumentOutOfRangeException();
                if (value == _audioGain) return;
                _audioGain = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VolumeAdjustment)));

                var request = new vx_req_aux_set_mic_level_t();
                request.level = ConvertGain(value);
                _client.BeginIssueRequest(request, ar =>
                {
                    try
                    {
                        _client.EndIssueRequest(ar);
                    }
                    catch (Exception e)
                    {
                        VivoxDebug.Instance.VxExceptionMessage($"{request.GetType().Name} failed: {e}");
                        if (VivoxDebug.Instance.throwInternalExcepetions)
                        {
                            throw;
                        }
                    }
                });
            }
        }

        public bool Muted
        {
            get { return _muted; }
            set
            {
                if (value == _muted) return;
                _muted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Muted)));
                var request = new vx_req_connector_mute_local_mic_t(); 
                request.mute_level = (value ? 1 : 0);
                _client.BeginIssueRequest(request, ar =>
                {
                    try
                    {
                        _client.EndIssueRequest(ar);
                    }
                    catch (Exception e)
                    {
                        VivoxDebug.Instance.VxExceptionMessage($"{request.GetType().Name} failed: {e}");
                        if (VivoxDebug.Instance.throwInternalExcepetions)
                        {
                            throw;
                        }
                    }
                });
            }
        }

        public IAsyncResult BeginRefresh(AsyncCallback callback)
        {
            AsyncNoResult result = new AsyncNoResult(callback);
            var request = new vx_req_aux_get_capture_devices_t();
            _client.BeginIssueRequest(request, ar =>
            {
                vx_resp_aux_get_capture_devices_t response;
                try
                {
                    response = _client.EndIssueRequest(ar);
                    var oldDevices = new ReadWriteDictionary<string, IAudioDevice, AudioDevice>();
                    bool devicesChanged = false;
                    if (response.count != _devices.Count)
                    {
                        devicesChanged = true;
                    }
                    for (int i = 0; i < _devices.Count; i++)
                    {
                        oldDevices[_devices.Keys.ElementAt(i)] = _devices.ElementAt(i);
                    }
                    _devices.Clear();
                    for (var i = 0; i < response.count; ++i)
                    {
                        var device = VivoxCoreInstance.get_device(i, response.capture_devices);
                        var id = device.device;
                        var name = device.display_name;
                        var newDevice = new AudioDevice { Key = id, Name = name };
                        //if an id that didn't previously exist
                        if (!oldDevices.Contains(newDevice))
                        {
                            devicesChanged = true;
                        }
                        //If an id that did previously exist but the device has now changed (such as setting a new device name in system settings)
                        else if (!oldDevices[id].Equals(newDevice))
                        {
                            devicesChanged = true;
                        }
                        _devices[id] = newDevice;
                    }
                    if(devicesChanged)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AvailableDevices))); 
                    }
                    oldDevices.Clear();

                    var currentSystemDevice = new AudioDevice
                    {
                        Key = response.default_capture_device.device,
                        Name = response.default_capture_device.display_name
                    };
                    _currentCommunicationDevice = new AudioDevice
                    {
                        Key = response.default_communication_capture_device.device,
                        Name = response.default_communication_capture_device.display_name
                    };
                    var effectiveDevice = new AudioDevice
                    {
                        Key = response.effective_capture_device.device,
                        Name = response.effective_capture_device.display_name,
                    };
                    if (!effectiveDevice.Equals(_effectiveDevice))
                    {
                        // Only fire the event if the effective device has truly changed.
                        _effectiveDevice = effectiveDevice;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EffectiveDevice)));
                    }
                    if (!currentSystemDevice.Equals(_currentSystemDevice))
                    {
                        // Only fire the event if the system device has truly changed.
                        _currentSystemDevice = currentSystemDevice;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemDevice)));
                    }
                    result.SetComplete();
                }
                catch (Exception e)
                {
                    VivoxDebug.Instance.VxExceptionMessage($"{request.GetType().Name} failed: {e}");
                    result.SetComplete(e);
                    if (VivoxDebug.Instance.throwInternalExcepetions)
                    {
                        throw;
                    }
                    return;
                }
            });
            return result;
        }

        public void EndRefresh(IAsyncResult result)
        {
            (result as AsyncNoResult)?.CheckForError();
        }

        #endregion

        private void OnEventMessageReceived(vx_evt_base_t eventMessage)
        {
            if (eventMessage.type == vx_event_type.evt_audio_device_hot_swap)
            {
                HandleDeviceHotSwap(eventMessage);
            }
        }

        private void HandleDeviceHotSwap(vx_evt_base_t eventMessage)
        {
            BeginRefresh(new AsyncCallback((IAsyncResult result) =>
            {
                try
                {
                    EndRefresh(result);
                }
                catch (Exception e)
                {
                    VivoxDebug.Instance.VxExceptionMessage($"BeginRefresh failed: {e}");
                    if (VivoxDebug.Instance.throwInternalExcepetions)
                    {
                       throw;
                    }
                }
            }));
        }

        public void Clear()
        {
            _devices.Clear();
            _activeDevice = _defaultSystemDevice;
            _effectiveDevice = null;
            _muted = false;
            _audioGain = 0;
        }
    }
}
