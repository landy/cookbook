import { Remoting_buildProxy_Z15584635, RemotingModule_withBaseUrl, RemotingModule_withRouteBuilder, RemotingModule_createApi } from "./.fable/Fable.Remoting.Client.7.2.0/Remoting.fs.js";
import { UsersService$reflection, Route_builder } from "../Shared/Users.fs.js";

export const usersService = (() => {
    let arg00;
    let options_1;
    const options = RemotingModule_createApi();
    options_1 = RemotingModule_withRouteBuilder(Route_builder, options);
    arg00 = RemotingModule_withBaseUrl(config.baseUrl, options_1);
    return Remoting_buildProxy_Z15584635(arg00, {
        ResolveType: UsersService$reflection,
    });
})();

