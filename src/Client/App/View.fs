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

let private useToolbarStyles = Styles.makeStyles(fun styles theme ->
    {|
        appBarTitle = styles.create [
            style.flexGrow 1
        ]
        contextMenu = styles.create [
            style.position.relative
            style.fontSize 16
            style.width (length.perc 100)
            style.marginLeft (theme.spacing 2)
        ]
    |}
)

let private useRootViewStyles : unit -> _ = Styles.makeStyles(fun styles theme ->
    let drawerWidth = 240
    {
        root = styles.create  [
            style.display.flex
            style.userSelect.none
        ]
        appBar = styles.create [
            style.zIndex (theme.zIndex.drawer + 1)
        ]
        drawer = styles.create [
            style.width (length.px drawerWidth)
            style.flexShrink 0
        ]
        drawerPaper = styles.create [
            style.width (length.px drawerWidth)
            // Example of breakpoint media queries
            style.inner theme.breakpoints.downXs [
                style.backgroundColor.red
            ]
        ]
        content = styles.create [
            style.flexGrow 1
            style.padding (theme.spacing 3)
        ]
        toolbar = styles.create [
            yield! theme.mixins.toolbar
        ]
    }
)

let Toolbar = React.functionComponent(fun ((model), dispatch) ->
    let c = useToolbarStyles ()
    Mui.toolbar [
        yield Mui.typography [
            typography.variant.h6
            typography.color.inherit'
            typography.children [
                Mui.link [
                    yield prop.text "Cookbook"
                    yield link.color.inherit'
                    yield! (Html.Props.routed Main)
                ]
            ]
            typography.classes.root c.appBarTitle
        ]
        yield Mui.link [
            yield prop.text "Login"
            yield! (Html.Props.routed Page.Login)
            yield button.color.inherit'
        ]
    ]
)

let private pageListItem (page,(title:string)) =
    Mui.listItem [
        yield prop.key title
        yield listItem.button true
        yield! Html.Props.routed page
        yield listItem.children [
            Mui.listItemIcon [(inboxIcon [])]
            Mui.listItemText title
        ]
    ]

let sidebarMenuLinks = [
    UsersList, "Users"
    UsersAdd, "New user"
]

let private drawer appStyles =
    Mui.drawer [
        drawer.variant.permanent
        drawer.classes.root appStyles.drawer
        drawer.classes.paper appStyles.drawerPaper
        drawer.children [
            Html.div [ prop.className appStyles.toolbar ]
            Mui.list [
                list.component' "nav"
                list.children (sidebarMenuLinks |> List.map pageListItem |> ofList)
            ]
        ]
    ]

let pageView page =
    match page with
    | Main ->
        Html.div "Main"
    | UsersList ->
        UsersList.View.render ()
    | _ -> Html.div "unknown page"

let mainView = React.functionComponent("MainView", fun currentPage ->
    let appStyles = useRootViewStyles ()
    Html.main [
        prop.className appStyles.content
        prop.children [
            Html.div [ prop.className appStyles.toolbar ]
            pageView currentPage
        ]
    ]
)

let loginView = React.functionComponent("LoginView", fun props ->
    let appStyles = useRootViewStyles ()

    Html.main [
        prop.className appStyles.content
        prop.children [
            Html.div [ prop.className appStyles.toolbar ]
            Login.View.render props
        ]
    ]
)

let main = React.functionComponent(fun (model, (dispatch:Msg -> unit)) ->
    let appStyles = useRootViewStyles ()
    let lightTheme = Styles.createMuiTheme([
        theme.palette.type'.light
        theme.palette.primary Colors.indigo
        theme.palette.secondary Colors.pink
      ])

    Mui.themeProvider [
        themeProvider.theme lightTheme
        themeProvider.children [
            Html.div [
                prop.className appStyles.root
                prop.children [
                    yield Mui.cssBaseline []
                    yield Mui.appBar [
                        appBar.classes.root appStyles.appBar
                        appBar.position.fixed'
                        appBar.children [
                            Toolbar(model, dispatch)
                        ]
                    ]
                    match model.CurrentPage with
                    | Login ->
                        let loginProps : Login.State.LoginPageProps =
                            { Login.State.handleNewToken = TokenChanged >> dispatch }
                        yield loginView loginProps
                    | _ ->
                        yield drawer appStyles
                        yield mainView model.CurrentPage
                ]
            ]
        ]
    ]
)

let render (state:Model) (dispatch: Msg -> unit) =
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children (main (state,dispatch))
    ]