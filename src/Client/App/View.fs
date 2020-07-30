module Cookbook.Client.App.View

open Fable.React
open Feliz
open Feliz.MaterialUI
open Feliz.MaterialUI.themeOverrides.theme.overrides
open Feliz.Router
open Feliz.MaterialUI
open Fable.MaterialUI.Icons

open Cookbook.Client.Router
open Cookbook.Client.Pages
open Domain
open Domain.Styles
open Feliz.Styles

module Html =
    module Props =
        let routed (p:Page) = [
            prop.href (p |> Page.toUrlSegments |> Router.formatPath)
            prop.onClick (Router.goToUrl)
        ]

    let aRouted (text:string) (p:Page) =
        Html.a [
            yield! Props.routed p
            prop.text text
        ]

//let RoutedLink : ReactElement = React.functionComponent("RoutedLink", fun {|p:Page; text:string |} ->
//    Html.a [
//        yield! Html.Props.routed p
//        prop.text text
//    ]
//)

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
//let pageView page =
//    match page with
//    | Main ->
//        Html.div "Main"
//    | UsersList ->
//        UsersList.View.render ()
//    | _ -> Html.div "unknown page"
//
//let mainView = React.functionComponent("MainView", fun currentPage ->
//    let appStyles = useRootViewStyles ()
//    Html.main [
//        prop.className appStyles.content
//        prop.children [
//            Html.div [ prop.className appStyles.toolbar ]
//            pageView currentPage
//        ]
//    ]
//)
//
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

let topbarStyles = Styles.makeStyles(fun styles theme ->
    {|
        root = styles.create [
            style.boxShadow.none
        ]
        flexGrow = styles.create [
            style.flexGrow 1
        ]
    |}
)

type TopBarProps = {
    OpenSidebar : unit -> unit
}
let TopBar = React.functionComponent(fun props ->
    let styles = topbarStyles ()
    Mui.appBar [
        prop.className styles.root
        prop.children [
            Mui.toolbar [
                Mui.typography [
                    typography.variant.h6
                    typography.color.inherit'
                    typography.children [
                        Mui.link [
                            yield prop.text "Cookbook"
                            yield link.color.inherit'
                            yield! (Html.Props.routed Main)
                        ]
                    ]
                ]
                Html.div [ prop.className styles.flexGrow ]
                Mui.hidden [
                    hidden.lgUp true
                    prop.children [
                        Mui.iconButton [
                            iconButton.color.inherit'
                            prop.onClick (fun _ -> props.OpenSidebar () )
                            prop.children (menuIcon [])
                        ]
                    ]
                ]
            ]
        ]
    ]
)

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

let sidebarNavStyles = Styles.makeStyles (fun styles theme ->
    {|
        item = styles.create [
            style.display.flex
            style.paddingTop 0
            style.paddingBottom 0
        ]
        button = styles.create [
            style.color Colors.blueGrey.``800``
            style.padding (10,8)
            style.justifyContent.flexStart
            style.textTransform.none
            style.width (length.percent 100)
            style.fontWeight (theme.typography.fontWeightMedium)
        ]
        icon = styles.create [
            style.width 24
            style.height 24
            style.display.flex
            style.alignItems.center
            style.marginRight (theme.spacing(1))
        ]
    |}
)


let SideBarNav = React.functionComponent (fun state ->
    let styles = sidebarNavStyles()
    let pages = [
        {|
            Title = "Users"
            Icon = peopleIcon [
                prop.className styles.icon
            ]
        |}
    ]

    Mui.list
        (pages
        |> List.map (fun p ->
            Mui.listItem [
                prop.className styles.item
                listItem.disableGutters true
                prop.children [
                    Mui.button [
                        prop.text p.Title
                        prop.className styles.button
                        button.startIcon p.Icon
                        yield! Html.Props.routed UsersList
                    ]
                ]
            ]
        ))
)

let sidebarStyles = Styles.makeStyles (fun styles theme ->
    {|
        drawer = styles.create [
            style.width 240
            style.inner theme.breakpoints.upLg [
                style.marginTop 64
                style.height (length.calc "100% - 64px")
            ]
        ]
        root = styles.create [
            style.backgroundColor color.white
            style.display.flex
            style.flexDirection.column
            style.height (length.percent 100)
            style.padding (theme.spacing(2))
        ]
        nav = styles.create [
            style.marginBottom (theme.spacing(2))
        ]
    |}
)

type SidebarProps = {
    IsOpen : bool
    CloseSidebar : unit -> unit
    Variant : IReactProperty
}
let SideBar = React.functionComponent (fun (props:SidebarProps) ->
    let styles = sidebarStyles()
    Mui.drawer [
        drawer.PaperProps [prop.className styles.drawer]
        drawer.anchor.left
        drawer.open' props.IsOpen
        drawer.onClose (fun _ -> props.CloseSidebar())
        props.Variant
        prop.children [
            Html.div [
                prop.className styles.root
                prop.children [
                    SideBarNav [
                        prop.className styles.nav
                    ]
                ]
            ]
        ]
    ]
)

let MainView = React.functionComponent(fun (state, (dispatch : Msg -> unit)) ->
    Html.div "msssi"
)

let private useRootViewStyles : unit -> _ = Styles.makeStyles(fun styles theme ->
    let drawerWidth = 240
    {
        root = styles.create  [
            style.paddingTop 56
            style.height (length.percent 100)
            style.inner theme.breakpoints.upSm [
                style.paddingTop 64
            ]
        ]
        shiftContent = styles.create [
            style.paddingLeft 240
        ]
        content = styles.create [
            style.height (length.percent 100)
        ]

    }
)

//https://github.com/devias-io/react-material-dashboard/blob/master/src/layouts/Main/Main.js
let main = React.functionComponent(fun (state, (dispatch: Msg -> unit)) ->
    let appStyles = useRootViewStyles ()
    let isDesktop = Hooks.useMediaQuery(fun theme -> theme.breakpoints.upLg)
    let sidebarOpened, setOpenSidebar = React.useState(false)

    let openSidebar = fun _ -> setOpenSidebar true
    let closeSidebar = fun _ -> setOpenSidebar false

    let shouldOpenSidebar = if isDesktop then true else sidebarOpened

    Html.div[
        prop.className appStyles.root
        prop.children [
            Mui.cssBaseline []
            TopBar { OpenSidebar = openSidebar }
            SideBar {
                IsOpen = shouldOpenSidebar
                CloseSidebar = closeSidebar
                Variant = if isDesktop then drawer.variant.persistent else drawer.variant.temporary
            }
            Html.main [
                prop.className appStyles.content
                prop.children (MainView (state,dispatch))
            ]
        ]
    ]

)

let mainWithTheme = React.functionComponent(fun (state, (dispatch: Msg -> unit)) ->
    let lightTheme = Styles.createMuiTheme([
        theme.palette.type'.light
        theme.palette.primary Colors.indigo
        theme.palette.secondary Colors.pink

    ])

    Mui.themeProvider [
        themeProvider.theme lightTheme
        themeProvider.children [(main (state,dispatch))]
    ]
)

let render (state:Model) (dispatch: Msg -> unit) =
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children (mainWithTheme (state,dispatch))
    ]