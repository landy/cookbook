module Cookbook.Client.App.View

open Cookbook.Client.Auth
open Cookbook.Client.Auth.Domain
open Cookbook.Shared.Users.Response
open Feliz
open Feliz.Bulma
open Feliz.Router

open Cookbook.Client.Router
open Cookbook.Client.Pages
//open Domain
open Cookbook.Client.Components
open Cookbook.Client.Components.Html


//let private useToolbarStyles = Styles.makeStyles(fun styles theme ->
//    {|
//        appBarTitle = styles.create [
//            style.flexGrow 1
//        ]
//        contextMenu = styles.create [
//            style.position.relative
//            style.fontSize 16
//            style.width (length.perc 100)
//            style.marginLeft (theme.spacing 2)
//        ]
//    |}
//)



//
//let Toolbar = React.functionComponent(fun ((model), dispatch) ->
//    let c = useToolbarStyles ()
//    Mui.toolbar [
//        yield Mui.typography [
//            typography.variant.h6
//            typography.color.inherit'
//            typography.children [
//                Mui.link [
//                    yield prop.text "Cookbook"
//                    yield link.color.inherit'
//                    yield! (Html.Props.routed Main)
//                ]
//            ]
//            typography.classes.root c.appBarTitle
//        ]
//        yield Mui.link [
//            yield prop.text "Login"
//            yield! (Html.Props.routed Page.Login)
//            yield button.color.inherit'
//        ]
//    ]
//)
//
//let private pageListItem (page,(title:string)) =
//    Mui.listItem [
//        yield prop.key title
//        yield listItem.button true
//        yield! Html.Props.routed page
//        yield listItem.children [
//            Mui.listItemIcon [(inboxIcon [])]
//            Mui.listItemText title
//        ]
//    ]
//
//let sidebarMenuLinks = [
//    UsersList, "Users"
//    UsersAdd, "New user"
//]
//
//let private drawer appStyles =
//    Mui.drawer [
//        drawer.variant.permanent
//        drawer.classes.root appStyles.drawer
//        drawer.classes.paper appStyles.drawerPaper
//        drawer.children [
//            Html.div [ prop.className appStyles.toolbar ]
//            Mui.list [
//                list.component' "nav"
//                list.children (sidebarMenuLinks |> List.map pageListItem |> ofList)
//            ]
//        ]
//    ]
//
//
//let private useRootViewStyles (isDesktop : bool) : unit -> _ = Styles.makeStyles(fun styles theme ->
//    {
//        root = styles.create  [
//            style.paddingTop 56
//            style.height (length.percent 100)
//            style.inner theme.breakpoints.upSm [
//                style.paddingTop 64
//            ]
//            if isDesktop then
//                style.paddingLeft 240
//        ]
//        content = styles.create [
//            style.height (length.percent 100)
//        ]
//    }
//)
//
//let pageView page =
//    match page with
//    | Main ->
//        Html.div "Main"
//    | UsersList ->
//        UsersList.View.render ()
//    | UsersAdd ->
//        UsersAdd.View.render ()
//    | _ -> Html.div "unknown page"
//
////
//let loginView = React.functionComponent("LoginView", fun props ->
//    let appStyles = useRootViewStyles ()
//
//    Html.main [
//        prop.className appStyles.content
//        prop.children [
//            Html.div [ prop.className appStyles.toolbar ]
//            Login.View.render props
//        ]
//    ]
//)
//
//let main = React.functionComponent(fun (model, (dispatch:Msg -> unit)) ->
//    let appStyles = useRootViewStyles ()
//    let lightTheme = Styles.createMuiTheme([
//        theme.palette.type'.light
//        theme.palette.primary Colors.indigo
//        theme.palette.secondary Colors.pink
//      ])
//
//    Mui.themeProvider [
//        themeProvider.theme lightTheme
//        themeProvider.children [
//            Html.div [
//                prop.className appStyles.root
//                prop.children [
//                    yield Mui.cssBaseline []
//                    yield Mui.appBar [
//                        appBar.classes.root appStyles.appBar
//                        appBar.position.fixed'
//                        appBar.children [
//                            Toolbar(model, dispatch)
//                        ]
//                    ]
//                    match model.CurrentPage with
//                    | Login ->
//                        let loginProps : Login.State.LoginPageProps =
//                            { Login.State.handleNewToken = TokenChanged >> dispatch }
//                        yield loginView loginProps
//                    | _ ->
//                        yield drawer appStyles
//                        yield mainView model.CurrentPage
//                ]
//            ]
//        ]
//    ]
//)
//
//
//
//let topbarStyles = Styles.makeStyles(fun styles theme ->
//    {|
//        root = styles.create [
//            style.boxShadow.none
//        ]
//        flexGrow = styles.create [
//            style.flexGrow 1
//        ]
//    |}
//)
//
//type TopBarProps = {
//    OpenSidebar : unit -> unit
//}
//let TopBar = React.functionComponent(fun props ->
//    let styles = topbarStyles ()
//    Mui.appBar [
//        prop.className styles.root
//        prop.children [
//            Mui.toolbar [
//                Mui.typography [
//                    typography.variant.h6
//                    typography.color.inherit'
//                    typography.children [
//                        Mui.link [
//                            yield prop.text "Cookbook"
//                            yield link.color.inherit'
//                            yield! (prop.routed Main)
//                        ]
//                    ]
//                ]
//                Html.div [ prop.className styles.flexGrow ]
//                Mui.hidden [
//                    hidden.lgUp true
//                    prop.children [
//                        Mui.iconButton [
//                            iconButton.color.inherit'
//                            prop.onClick (fun _ -> props.OpenSidebar () )
//                            prop.children (menuIcon [])
//                        ]
//                    ]
//                ]
//            ]
//        ]
//    ]
//)

(*
button: {
    color: colors.blueGrey[800],
    padding: '10px 8px',
    justifyContent: 'flex-start',
    textTransform: 'none',
    letterSpacing: 0,
    width: '100%',
    fontWeight: theme.typography.fontWeightMedium
  },
*)
//
//let sidebarNavStyles = Styles.makeStyles (fun styles theme ->
//    {|
//        item = styles.create [
//            style.display.flex
//            style.paddingTop 0
//            style.paddingBottom 0
//        ]
//        button = styles.create [
//            style.color Colors.blueGrey.``800``
//            style.padding (10,8)
//            style.justifyContent.flexStart
//            style.textTransform.none
//            style.width (length.percent 100)
//            style.fontWeight (theme.typography.fontWeightMedium)
//        ]
//        icon = styles.create [
//            style.width 24
//            style.height 24
//            style.display.flex
//            style.alignItems.center
//            style.marginRight (theme.spacing(1))
//        ]
//    |}
//)
//
//
//let SideBarNav = React.functionComponent (fun state ->
//    let styles = sidebarNavStyles()
//    let pages = [
//        {|
//            Title = "Users"
//            Icon = peopleIcon [
//                prop.className styles.icon
//            ]
//        |}
//    ]
//
//    Mui.list
//        (pages
//        |> List.map (fun p ->
//            Mui.listItem [
//                prop.className styles.item
//                listItem.disableGutters true
//                prop.children [
//                    Mui.button [
//                        prop.text p.Title
//                        prop.className styles.button
//                        button.startIcon p.Icon
//                        yield! prop.routed UsersList
//                    ]
//                ]
//            ]
//        ))
//)
//
//let sidebarStyles = Styles.makeStyles (fun styles theme ->
//    {|
//        drawer = styles.create [
//            style.width 240
//            style.inner theme.breakpoints.upLg [
//                style.marginTop 64
//                style.height (length.calc "100% - 64px")
//            ]
//        ]
//        root = styles.create [
//            style.backgroundColor color.white
//            style.display.flex
//            style.flexDirection.column
//            style.height (length.percent 100)
//            style.padding (theme.spacing(2))
//        ]
//        nav = styles.create [
//            style.marginBottom (theme.spacing(2))
//        ]
//    |}
//)
//
//type SidebarProps = {
//    IsOpen : bool
//    CloseSidebar : unit -> unit
//    Variant : IReactProperty
//}
//let SideBar = React.functionComponent (fun (props:SidebarProps) ->
//    let styles = sidebarStyles()
//    Mui.drawer [
//        drawer.PaperProps [prop.className styles.drawer]
//        drawer.anchor.left
//        drawer.open' props.IsOpen
//        drawer.onClose (fun _ -> props.CloseSidebar())
//        props.Variant
//        prop.children [
//            Html.div [
//                prop.className styles.root
//                prop.children [
//                    SideBarNav [
//                        prop.className styles.nav
//                    ]
//                ]
//            ]
//        ]
//    ]
//)
//
//let useLoginStyles = Styles.makeStyles (fun styles theme ->
//    {|
//        content = styles.create []
//
//    |}
//)
//
//let loginView = React.functionComponent("LoginView", fun props ->
//    let styles = useLoginStyles ()
//
//    Html.main [
//        prop.className styles.content
//        prop.children [
//
//            Login.View.render props
//        ]
//    ]
//)
//
//
////https://github.com/devias-io/react-material-dashboard/blob/master/src/layouts/Main/Main.js
//let main = React.functionComponent(fun (state, (dispatch: Msg -> unit)) ->
//    let isDesktop = Hooks.useMediaQuery(fun theme -> theme.breakpoints.upLg)
//
//    let appStyles = useRootViewStyles isDesktop ()
//    let sidebarOpened, setOpenSidebar = React.useState(false)
//
//    let openSidebar = fun _ -> setOpenSidebar true
//    let closeSidebar = fun _ -> setOpenSidebar false
//
//    let shouldOpenSidebar = if isDesktop then true else sidebarOpened
//
//    Html.div[
//        prop.className appStyles.root
//        prop.children
//            [
//                Mui.cssBaseline []
//                TopBar { OpenSidebar = openSidebar }
//                match state.CurrentPage with
//                | Login ->
//                    let loginProps : Login.State.LoginPageProps =
//                        { Login.State.handleNewToken = TokenChanged >> dispatch }
//                    loginView loginProps
//                | _ ->
//                    SideBar {
//                        IsOpen = shouldOpenSidebar
//                        CloseSidebar = closeSidebar
//                        Variant = if isDesktop then drawer.variant.persistent else drawer.variant.temporary
//                    }
//                    Html.main [
//                        prop.className appStyles.content
//                        prop.children (pageView state.CurrentPage)
//                    ]
//            ]
//    ]
//
//)
//
//
//

[<ReactComponent>]
let NavbarMenuItem page (label:string) closeBurgerMenu (children: ReactElement list) =
    let url = page |> Page.toUrlSegments |> Router.formatPath
    let currentUrl = Router.currentPath () |> Router.formatPath

    Bulma.navbarItem.div [
        prop.key currentUrl
        if children |> List.isEmpty |> not then
            yield!
                [
                    navbarItem.hasDropdown
                    navbarItem.isHoverable
                ]

        prop.children [
            Bulma.navbarLink.a [
                prop.href url
                prop.text label
                prop.onClick (fun e ->
                    closeBurgerMenu ()
                    Router.goToUrl e
                )
            ]
            if children |> List.isEmpty |> not then
                Bulma.navbarDropdown.div [
                    yield! children
                ]
        ]
    ]

[<ReactComponent>]
let NavbarLink page (label: string) closeBurgerMenu =
    let url = page |> Page.toUrlSegments |> Router.formatPath
    Bulma.navbarItem.a [
        prop.href url
        prop.text label
        prop.onClick (fun e ->
            closeBurgerMenu ()
            Router.goToUrl e
        )
    ]

[<ReactComponent>]
let TopNavBar (page:Page) =
    let isActive,setIsActive = React.useState(false)
    let closeMenu = (fun  _ -> false |> setIsActive)
    Bulma.navbar [
        color.isPrimary
        navbar.hasShadow
        prop.children [
            Bulma.container[
                Bulma.navbarBrand.div [
                    Bulma.navbarItem.a [
                        size.isSize3
                        yield! prop.routed Page.Main
                        prop.text "HHM"
                    ]
                    Bulma.navbarBurger [
                        if isActive then navbarBurger.isActive
                        prop.onClick (fun e -> e.preventDefault(); isActive |> not |> setIsActive)
                        prop.children [
                            Html.span [ prop.ariaHidden true ]
                            Html.span [ prop.ariaHidden true ]
                            Html.span [ prop.ariaHidden true ]
                        ]
                    ]
                ]
                Bulma.navbarMenu [
                    if isActive then navbarMenu.isActive
                    prop.children [
                        Bulma.navbarStart.div [
                            NavbarMenuItem Page.UsersList "Cookbook" closeMenu [
                                NavbarLink Page.UsersAdd "Add User" closeMenu
                            ]
                        ]
                        Bulma.navbarEnd.div [
                            Bulma.dropdownDivider []
                            NavbarLink Page.Login "Login" closeMenu
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let MainContent () =
    let auth = React.useContext(Context.authContext)
    Bulma.section[
        spacing.pt3
        prop.children [
            Bulma.field.div [
                                Bulma.fieldLabel "test"
                                Bulma.fieldBody [
                                    Bulma.input.text [
                                        prop.name "test"
                                    ]
                                ]
                            ]
            Bulma.container (sprintf "User: %A"  auth.CurrentUser)

        ]

    ]

[<ReactComponent>]
let private LoginContent () =
    let auth = React.useContext(Context.authContext)
    Bulma.hero [
        color.isPrimary
        hero.isFullHeight
        prop.children [
            Bulma.heroBody [
                Bulma.container [
                    text.hasTextCentered
                    prop.children [
                        Bulma.column [
                            column.is4
                            column.isOffset4
                            prop.children [
                                Cookbook.Client.Pages.Login.View.render auth.SetUser
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let MainView page =
    React.fragment [
        TopNavBar page
        MainContent ()
    ]

[<ReactComponent>]
let GetTemplate page =
    match page with
    | Login ->
        LoginContent ()
    | UsersAdd ->
        MainView page
    | UsersList ->
        MainView page
    | _ ->
        MainView page


[<ReactComponent>]
let MainApplication () =
    let currentPage = Router.currentPath () |> Page.parseFromUrlSegments
    let page,setPage = React.useState(currentPage)
    let (user: UserSession option),setUser = React.useState(None)

    React.useEffectOnce(fun () ->
        AuthStorage.tryGetSession()
        |> setUser
    )

    let ctx = {
        CurrentUser = user
        SetUser = (fun u ->
            AuthStorage.save u
            u |> Some |> setUser)
        Logout = (fun () -> setUser None)
    }

    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> setPage)
        router.children [
            (Cookbook.Client.Auth.Context.AuthContext ctx (GetTemplate page))
        ]
    ]